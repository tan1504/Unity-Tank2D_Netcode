using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticated,
    Error,
    TimeOut
}

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

    public static async Task<AuthState> DoAuth(int maxRetries = 5)
    {
        if (AuthState == AuthState.Authenticated)
            return AuthState;

        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating!");
            await Authenticating();
            return AuthState;
        }

        await SignInAnonymouslyAsync(maxRetries);

        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);      // wait for 0.2s
        }

        return AuthState;
    }

    private static async Task SignInAnonymouslyAsync(int maxRetries = 5)
    {
		AuthState = AuthState.Authenticating;

		int retries = 0;
		while (AuthState == AuthState.Authenticating && retries < maxRetries)
		{
            try
            {
				await AuthenticationService.Instance.SignInAnonymouslyAsync();

				if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
				{
					AuthState = AuthState.Authenticated;
					break;
				}
			}
            catch (AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException requestException)   // If we dont have Internet connection
            {
                Debug.LogError(requestException);
                AuthState = AuthState.Error;
            }

			retries++;
			await Task.Delay(1000);     // wait for 1s

            if (AuthState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player was not signed in successfully after {retries} retries");
                AuthState = AuthState.TimeOut;
            }
		}
	}
}

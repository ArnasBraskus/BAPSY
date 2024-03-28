using Google.Apis.Auth;

public class GoogleTokenValidator {
    private GoogleJsonWebSignature.ValidationSettings GoogleValidationSettings;

    public GoogleTokenValidator(string googleClientId) {
        GoogleValidationSettings = new GoogleJsonWebSignature.ValidationSettings() { Audience = new string[] { googleClientId } };
    }

    public GoogleTokenValidator() {

    }

    public virtual GoogleJsonWebSignature.Payload ValidateToken(string token) {
        return GoogleJsonWebSignature.ValidateAsync(token, GoogleValidationSettings).Result;
    }
}

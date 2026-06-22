# Security Standards

Authentication:
- JWT
- OAuth2/OIDC compatible
- Refresh token rotation

Authorization:
- Policy-based authorization
- Role + claim-based access

Security Rules:
- Never trust client input
- Validate all requests
- Use HTTPS only
- Use secure cookies

Passwords:
- Hash with BCrypt/Argon2

MFA:
- Extensible MFA support

Audit:
- Audit critical actions
- Store trace metadata

Avoid:
- Plaintext secrets
- Hardcoded credentials
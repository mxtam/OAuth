const authSettings = {
    authority: 'https://localhost:7000',
    client_id: 'react-client',
    redirect_uri: 'http://localhost:5173/oauth/callback',
    silent_redirect_uri: 'http://localhost:5173/oauth/callback',
    post_logout_redirect_uri: 'http://localhost:5173/',
    response_type: 'code',
    automaticSilentRenew: true,
    scope: 'api1 openid profile offline_access'
};

export const authConfig = {
    settings: authSettings,
    flow: 'authentication'
};
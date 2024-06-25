const authSettings = {
    authority: 'https://localhost:7000',
    client_id: 'react-client',
    client_secret: '901564A5-E7FE-42CB-B10D-61EF6A8F3654',
    redirect_uri: 'http://localhost:3000/oauth/callback',
    silent_redirect_uri: 'http://localhost:3000/oauth/callback',
    post_logout_redirect_uri: 'http://localhost:3000/',
    response_type: 'code',
    scope: 'api1'
};

export const authConfig = {
    settings: authSettings,
    flow: 'authentication'
};
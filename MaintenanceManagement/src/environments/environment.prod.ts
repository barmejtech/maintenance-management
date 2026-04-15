export const environment = {
  production: true,
  apiUrl: '/api',
  // Empty string resolves to the current origin; nginx proxies /api and /hubs to the backend
  baseUrl: '',
  hubsUrl: ''
};

// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  authApi: 'http://localhost:1000/api/v1/auth/',
  customerApi: 'http://localhost:2000/api/v1/customers/',
  productApi: 'http://localhost:4000/api/v1/products/',
  manufacturerApi: 'http://localhost:3000/api/v1/manufacturers/',
  userApi: 'http://localhost:7000/api/v1/users/',
  logApi: 'http://localhost:8000/api/v1/logs/'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.

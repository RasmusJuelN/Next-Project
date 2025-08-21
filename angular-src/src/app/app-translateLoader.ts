// import { HttpClient, HttpClientModule } from '@angular/common/http';
// import { NgModule } from '@angular/core';
// import { BrowserModule } from '@angular/platform-browser';
// import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
// import { TranslateHttpLoader } from '@ngx-translate/http-loader';
// import { AppComponent } from './app.component';
// import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

// export function HttpLoaderFactory(http: HttpClient) {
//   // In v17, TranslateHttpLoader accepts ONLY the HttpClient now
//   return new TranslateHttpLoader();
// }

// @NgModule({
//   // declarations: [AppComponent],
//   imports:[
//     BrowserModule,
//     TranslateModule.forRoot({
//       loader: {
//         provide: TranslateLoader,
//         useFactory: HttpLoaderFactory,
//         deps: [HttpClient]
//       }
//     })
//   ],
//   providers: [TranslateService],
//   bootstrap: [AppComponent]
// })
// class AppModule {}

// platformBrowserDynamic().bootstrapModule(AppModule)
//   .catch(err => console.error(err));
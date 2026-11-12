import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { APP_CONFIG, AppConfig } from './app/app-config';

fetch('/config.json')
  .then((response) => {
    if (!response.ok) {
      throw new Error('config.json was not found');
    }

    return response.json() as Promise<AppConfig>;
  })
  .then((config) =>
    bootstrapApplication(App, {
      providers: [{ provide: APP_CONFIG, useValue: config }, ...appConfig.providers],
    }),
  )
  .catch((err) => console.error(err));

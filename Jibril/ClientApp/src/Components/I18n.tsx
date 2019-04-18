import I18n from 'i18next';
import * as XHR from 'i18next-xhr-backend';
import * as LDetect from 'i18next-browser-languagedetector';
import { load } from 'js-yaml';

const options: I18n.InitOptions = {
  fallbackLng: 'en',
  load: 'languageOnly',
  debug: true,
  saveMissing: true,
  backend: {
    loadPath: '/locales/{{lng}}.yaml',
    allowMultiLoading: false,
    crossDomain: false,
    parse: load
  },
  interpolation: {
    escapeValue: false,
  }
};

I18n
  .use(LDetect)
  .use(XHR);

if (!I18n.isInitialized)
  I18n.init(options);

export default I18n;
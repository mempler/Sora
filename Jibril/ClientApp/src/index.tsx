import * as React from 'react';
import { render }from 'react-dom';
import * as ServiceWorker from './Typescript/React/serviceWorker';

import i18n from './Components/I18n';
import App from './Components/App';

i18n.on('initialized', () => {
  render(<App/>, document.getElementById('root'));
  ServiceWorker.register({ });
})

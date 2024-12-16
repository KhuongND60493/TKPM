import {Provider} from 'react-redux';
import {createRoot} from 'react-dom/client';
import {ThemeProvider} from '@emotion/react';
import {CssBaseline} from '@mui/material';
import theme from '@/features/theme';
import {store, persistor} from '@/app/store'

import App from '@/app';

import {PersistGate} from 'redux-persist/integration/react';
// // import '@/assets/scss/style.scss';
import {Toaster} from 'react-hot-toast';

const container = document.getElementById('root');
const root = createRoot(container!);

const AppRoot = () => {
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline/>
            <App/>
        </ThemeProvider>
    );
};
const app = (
    <Provider store={store}>
        <PersistGate loading={null} persistor={persistor}>
            <AppRoot/>
            <Toaster/>
        </PersistGate>
    </Provider>
);

root.render(app);

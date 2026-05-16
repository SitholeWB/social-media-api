import * as React from 'react';
import { ThemeProvider, createTheme } from '@mui/material/styles';
import type { ThemeOptions } from '@mui/material/styles';
import { inputsCustomizations } from './customizations/inputs';
import { dataDisplayCustomizations } from './customizations/dataDisplay';
import { feedbackCustomizations } from './customizations/feedback';
import { navigationCustomizations } from './customizations/navigation';
import { surfacesCustomizations } from './customizations/surfaces';
import { colorSchemes, typography, shadows, shape } from './themePrimitives';
import { useTenant } from '../contexts/TenantContext';

interface AppThemeProps {
	children: React.ReactNode;
	/**
	 * This is for the docs site. You can ignore it or remove it.
	 */
	disableCustomTheme?: boolean;
	themeComponents?: ThemeOptions['components'];
}

export default function AppTheme(props: AppThemeProps) {
	const { children, disableCustomTheme, themeComponents } = props;
	const { themeConfig } = useTenant();
	const theme = React.useMemo(() => {
		return disableCustomTheme
			? {}
			: createTheme({
				// For more details about CSS variables configuration, see https://mui.com/material-ui/customization/css-theme-variables/configuration/
				cssVariables: {
					colorSchemeSelector: 'data-mui-color-scheme',
					cssVarPrefix: 'template',
				},
				colorSchemes: {
					...colorSchemes,
					light: {
						...colorSchemes?.light,
						palette: {
							...colorSchemes?.light?.palette,
							primary: {
								...colorSchemes?.light?.palette?.primary,
								...(themeConfig?.primaryColor && { main: themeConfig.primaryColor })
							},
							secondary: {
								...colorSchemes?.light?.palette?.secondary,
								...(themeConfig?.secondaryColor && { main: themeConfig.secondaryColor })
							}
						}
					}
				},
				typography: {
					...typography,
					...(themeConfig?.fontFamily && { fontFamily: themeConfig.fontFamily })
				},
				shadows,
				shape,
				components: {
					...inputsCustomizations,
					...dataDisplayCustomizations,
					...feedbackCustomizations,
					...navigationCustomizations,
					...surfacesCustomizations,
					...themeComponents,
				},
			});
	}, [disableCustomTheme, themeComponents, themeConfig]);
	if (disableCustomTheme) {
		return <React.Fragment>{children}</React.Fragment>;
	}
	return (
		<ThemeProvider theme={theme} disableTransitionOnChange>
			{children}
		</ThemeProvider>
	);
}
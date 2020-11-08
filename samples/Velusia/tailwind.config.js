const defaultTheme = require('tailwindcss/defaultTheme')

module.exports = {
	purge: {
		enabled: process.env.NODE_ENV === 'production',
		content: ['./Velusia.Server/Views/**/*.cshtml']
	},
	theme: {
		extend: {
			fontFamily: {
				sans: ['Inter var', ...defaultTheme.fontFamily.sans],
			}
		},
		typography: {
			default: {
			  css: {
				'pre code::before': {
					content: 'none',
				},
				'pre code::after': {
					content: 'none',
				},
			  },
			},
		},
	},
	variants: {
	},
	plugins: [
		require('@tailwindcss/typography')
	]
};
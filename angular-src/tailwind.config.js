/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}", // Important: ensure Tailwind scans Angular templates
  ],
  theme: {
    extend: {
      colors: {
        primary_background_white: '#ffffff', // off-white background
        secondary_background_gray: '#F5F5F5', // light gray background
        secondary_background_hover: '#EBEBEB', // hover effect for gray backgrounds
        
        primary_orange_1: '#e94f2d',
        primary_orange_light: '#FF6B3D', // lighter orange
        primary_orange_dark: '#b32d13', // darker orange'
        primary_orange_darker: "#260700",

        primary_dark_blue: '#000c2e', //dark blue
        primary_dark_light: '#283361', // lighter dark
        primary_dark_dark: '#00061a', // even darker

        secondary_blue: '#2563eb', // accent blue
        secondary_gray: '#64748b', // accent gray
        accent_yellow: '#fbbf24', // accent yellow
        accent_green: '#22c55e', // accent green
        accent_red: '#ef4444', // accent red
        // Add more custom colors here
      },
    },
  },
  plugins: [],
}


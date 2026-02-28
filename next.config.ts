import type { NextConfig } from 'next';

const nextConfig: NextConfig = {
  // Exclude legacy vanilla JS/CSS from compilation
  webpack: (config) => {
    config.module?.rules?.push({
      test: /src[\\/](js|css)[\\/]/,
      use: 'null-loader',
    });
    return config;
  },
};

export default nextConfig;

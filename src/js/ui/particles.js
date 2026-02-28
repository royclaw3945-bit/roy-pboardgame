// Trickerion UI - Particle Effects (tsParticles)

export function createParticleMethods(ui) {
  return {
    initParticles() {
      const checkInterval = setInterval(() => {
        if (typeof tsParticles !== 'undefined') {
          clearInterval(checkInterval);
          ui.setupTitleParticles();
        }
      }, 200);
      setTimeout(() => clearInterval(checkInterval), 5000);
    },

    setupTitleParticles() {
      if (ui.particlesInitialized) return;
      try {
        tsParticles.load('particles-js', {
          fullScreen: false,
          particles: {
            number: { value: 30, density: { enable: true, area: 800 } },
            color: { value: ['#D4A853', '#B87333', '#CD9B1D', '#8C7853'] },
            shape: { type: 'circle' },
            opacity: {
              value: 0.3,
              random: true,
              animation: { enable: true, speed: 0.5, minimumValue: 0.05, sync: false }
            },
            size: {
              value: 3,
              random: { enable: true, minimumValue: 1 },
              animation: { enable: true, speed: 1, minimumValue: 0.5, sync: false }
            },
            move: {
              enable: true,
              speed: 0.5,
              direction: 'top',
              random: true,
              straight: false,
              outModes: { default: 'out' }
            },
            links: { enable: false }
          },
          detectRetina: true
        });
        ui.particlesInitialized = true;
      } catch (e) { /* skip */ }
    },

    switchToGameParticles() {
      if (typeof tsParticles === 'undefined') return;
      try {
        tsParticles.load('particles-js', {
          fullScreen: false,
          particles: {
            number: { value: 15, density: { enable: true, area: 1200 } },
            color: { value: ['#D4A853', '#B87333'] },
            shape: { type: 'circle' },
            opacity: {
              value: 0.12,
              random: true,
              animation: { enable: true, speed: 0.3, minimumValue: 0.02, sync: false }
            },
            size: {
              value: 2,
              random: { enable: true, minimumValue: 0.5 }
            },
            move: {
              enable: true,
              speed: 0.3,
              direction: 'top',
              random: true,
              straight: false,
              outModes: { default: 'out' }
            },
            links: { enable: false }
          },
          detectRetina: true
        });
      } catch (e) { /* skip */ }
    },

    triggerCelebrationParticles() {
      if (typeof tsParticles === 'undefined') return;
      try {
        tsParticles.load('particles-js', {
          fullScreen: false,
          particles: {
            number: { value: 80 },
            color: { value: ['#D4A853', '#F0D48A', '#B87333', '#CD9B1D', '#fff'] },
            shape: { type: ['circle', 'square'] },
            opacity: {
              value: 0.8,
              animation: { enable: true, speed: 1, minimumValue: 0 }
            },
            size: {
              value: 4,
              random: { enable: true, minimumValue: 1 }
            },
            move: {
              enable: true,
              speed: 3,
              direction: 'none',
              random: true,
              straight: false,
              gravity: { enable: true, acceleration: 2 },
              outModes: { default: 'destroy' }
            },
            links: { enable: false }
          },
          detectRetina: true
        });
      } catch (e) { /* skip */ }
    }
  };
}

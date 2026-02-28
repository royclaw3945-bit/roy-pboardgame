// Trickerion UI - GSAP Animation Helpers

export function createAnimationMethods(ui) {
  function gsapOk() { return typeof gsap !== 'undefined'; }

  return {
    gsapAvailable() { return gsapOk(); },

    animateScreenTransition(screenId) {
      if (!gsapOk()) return;
      const el = document.getElementById(screenId);
      if (!el) return;
      gsap.fromTo(el,
        { opacity: 0, y: 20 },
        { opacity: 1, y: 0, duration: 0.5, ease: 'power2.out' }
      );
    },

    animateTitleEntrance() {
      if (!gsapOk()) return;
      const tl = gsap.timeline();

      tl.fromTo('.title-logo-icon',
        { scale: 0, rotation: -180 },
        { scale: 1, rotation: 0, duration: 0.8, ease: 'back.out(1.7)' }
      );
      tl.fromTo('.title-main',
        { y: 30, opacity: 0, letterSpacing: '15px' },
        { y: 0, opacity: 1, letterSpacing: '6px', duration: 0.7, ease: 'power3.out' },
        '-=0.3'
      );
      tl.fromTo('.title-ornament',
        { scaleX: 0, opacity: 0 },
        { scaleX: 1, opacity: 1, duration: 0.4, ease: 'power2.out' },
        '-=0.2'
      );
      tl.fromTo('.subtitle',
        { y: 15, opacity: 0 },
        { y: 0, opacity: 1, duration: 0.4, ease: 'power2.out' },
        '-=0.1'
      );
      tl.fromTo('.title-desc',
        { y: 10, opacity: 0 },
        { y: 0, opacity: 1, duration: 0.4, ease: 'power2.out' },
        '-=0.1'
      );
      tl.fromTo('.title-buttons .btn',
        { y: 15, opacity: 0, scale: 0.9 },
        { y: 0, opacity: 1, scale: 1, duration: 0.4, ease: 'back.out(1.5)', stagger: 0.1 },
        '-=0.1'
      );
    },

    animateModalOpen() {
      if (!gsapOk()) return;
      gsap.fromTo('#modal',
        { y: 30, scale: 0.95, opacity: 0 },
        { y: 0, scale: 1, opacity: 1, duration: 0.35, ease: 'back.out(1.3)' }
      );
    },

    animateModalClose(callback) {
      if (!gsapOk()) { if (callback) callback(); return; }
      gsap.to('#modal', {
        y: 15, scale: 0.97, opacity: 0,
        duration: 0.2, ease: 'power2.in', onComplete: callback
      });
    },

    animateScoreChange(elementId) {
      if (!gsapOk()) return;
      const el = document.getElementById(elementId);
      if (!el) return;
      gsap.fromTo(el,
        { scale: 1.4, color: '#F0D48A' },
        { scale: 1, color: '', duration: 0.4, ease: 'back.out(2)' }
      );
    },

    animateRoundBanner() {
      if (!gsapOk()) return;
      const banner = document.querySelector('.phase-banner');
      if (!banner) return;
      gsap.fromTo(banner,
        { y: -20, opacity: 0, scale: 0.95 },
        { y: 0, opacity: 1, scale: 1, duration: 0.5, ease: 'back.out(1.3)' }
      );
    }
  };
}

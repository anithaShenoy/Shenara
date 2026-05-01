import React, { useEffect, useMemo, useRef, useState } from 'react';
import { createRoot } from 'react-dom/client';
import './styles.css';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5189/api';
const ADMIN_TOKEN_KEY = 'shenara-admin-token';
const INQUIRIES_VIEWED_KEY = 'shenara-inquiries-last-viewed';
const LOGO_URL = '/assets/Shenara Logo.jpg';

const fallbackServices = [
  {
    id: 1,
    name: 'Balloon Decor',
    description: 'Custom garlands, clusters, and color palettes styled around your celebration.',
    imageUrl: '/assets/Balloon%20Decor/5.jpg',
    startingPrice: 'From $45',
    isFeatured: true
  },
  {
    id: 2,
    name: 'Back Arch Styling',
    description: 'Statement arches with fabric, florals, signage, and premium balloon layering.',
    imageUrl: '/assets/Back%20arch%20styling/2.jpg',
    startingPrice: 'From $220',
    isFeatured: true
  },
  {
    id: 3,
    name: 'Custom Event Setups',
    description: 'Personalized decor concepts for birthdays, showers, engagements, and intimate events.',
    imageUrl: 'https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=1200&q=80',
    startingPrice: 'Custom quote',
    isFeatured: true
  }
];

const galleryCollections = [
  {
    id: 'baby-shower',
    title: 'Baby Shower',
    eventType: 'Soft celebration styling',
    imageUrl: '/assets/Gallery/Baby%20Shower/1000044844.jpg',
    summary: 'Gentle balloon styling and a sweet focal setup for welcoming a little one.',
    items: [
      {
        title: 'Sweet arrival backdrop',
        description: 'A soft baby shower moment with a calm palette, rounded balloon volume, and a photo-friendly focal point for family memories.',
        mediaUrl: '/assets/Gallery/Baby%20Shower/1000044844.jpg',
        type: 'image'
      }
    ]
  },
  {
    id: 'birthday-decor',
    title: 'Birthday Decor',
    eventType: 'Birthday styling',
    imageUrl: '/assets/Gallery/Birthday%20Decor/1000038891.jpg',
    summary: 'Playful birthday setups with personalized color stories and celebratory balloon details.',
    items: [
      {
        title: 'Personal birthday feature',
        description: 'A polished birthday setup designed around a clear focal wall, soft balloon clusters, and space for cake table styling.',
        mediaUrl: '/assets/Gallery/Birthday%20Decor/1000038891.jpg',
        type: 'image'
      },
      {
        title: 'Layered celebration palette',
        description: 'A cheerful balloon arrangement with balanced color placement, ideal for photos, guest arrivals, and party table moments.',
        mediaUrl: '/assets/Gallery/Birthday%20Decor/1000043892.jpg',
        type: 'image'
      },
      {
        title: 'Statement birthday scene',
        description: 'A fuller birthday decor look with height, dimension, and a finished backdrop that makes the celebration feel complete.',
        mediaUrl: '/assets/Gallery/Birthday%20Decor/1000053724.jpg',
        type: 'image'
      }
    ]
  },
  {
    id: 'special-occasions',
    title: 'Special Occasions',
    eventType: 'Custom event styling',
    imageUrl: '/assets/Gallery/Special%20Occasions/1000044566.jpg',
    summary: 'Custom styling for milestones, intimate gatherings, and photo-ready celebration moments.',
    items: [
      {
        title: 'Milestone celebration setup',
        description: 'A refined occasion backdrop with coordinated balloon styling and a clean event-ready composition.',
        mediaUrl: '/assets/Gallery/Special%20Occasions/1000044566.jpg',
        type: 'image'
      },
      {
        title: 'Elegant detail moment',
        description: 'A styled decor detail that brings texture, shape, and warmth into the overall celebration story.',
        mediaUrl: '/assets/Gallery/Special%20Occasions/1000048509.png',
        type: 'image'
      },
      {
        title: 'Room-ready event motion',
        description: 'A short event view showing how the decor reads in the room, with volume, color, and spacing working together.',
        mediaUrl: '/assets/Gallery/Special%20Occasions/1000048525.mp4',
        type: 'video'
      },
      {
        title: 'Walkthrough styling view',
        description: 'A moving look at the setup that helps customers understand the full scale and flow of the decor moment.',
        mediaUrl: '/assets/Gallery/Special%20Occasions/1000048633.mp4',
        type: 'video'
      },
      {
        title: 'Custom occasion accent',
        description: 'A tailored decor accent with a polished finish, ideal for milestone photos and intimate celebration spaces.',
        mediaUrl: '/assets/Gallery/Special%20Occasions/1000050503.png',
        type: 'image'
      },
      {
        title: 'Finished celebration scene',
        description: 'A complete special occasion setup with a balanced focal point, thoughtful styling, and guest-ready presentation.',
        mediaUrl: '/assets/Gallery/Special%20Occasions/1000053727.jpg',
        type: 'image'
      }
    ]
  }
];

const serviceDetailContent = {
  'balloon-decor': {
    title: 'Balloon Decor',
    eyebrow: 'Balloon Decor',
    intro: 'Layered balloon styling designed to feel tailored to the celebration, the venue, and the photographs you want to remember.',
    highlights: [
      {
        title: 'Celebration-ready balloon wall',
        description: 'A full balloon moment with soft volume, playful movement, and a polished focal point for birthdays, showers, and guest photos.',
        imageUrl: '/assets/Balloon%20Decor/5.jpg'
      },
      {
        title: 'Layered garland styling',
        description: 'Balloon clusters are arranged with intentional sizing, color rhythm, and height so the setup feels full without overwhelming the room.',
        imageUrl: '/assets/Balloon%20Decor/6.jpg'
      },
      {
        title: 'Theme-matched details',
        description: 'Colors, shapes, and accent placement can be coordinated with the event theme, dessert table, signage, and room layout.',
        imageUrl: '/assets/Balloon%20Decor/7.jpg'
      },
      {
        title: 'Photo-friendly color balance',
        description: 'Balanced tones and sculpted form help the decor read beautifully in person and stay crisp in every celebration photo.',
        imageUrl: '/assets/Balloon%20Decor/8.jpg'
      }
    ]
  },
  'back-arch-styling': {
    title: 'Back Arch Styling',
    eyebrow: 'Back Arch Styling',
    intro: 'Structured arch moments styled with fabric, balloons, signage, and soft detail work for birthdays, showers, engagements, and event entrances.',
    highlights: [
      {
        title: 'Statement focal arch',
        description: 'A shaped back arch gives the event a clear visual anchor for cake tables, welcome areas, and keepsake photos.',
        imageUrl: '/assets/Back%20arch%20styling/2.jpg'
      },
      {
        title: 'Soft layered styling',
        description: 'Fabric, balloons, and structure are composed in layers so the backdrop feels dimensional, elegant, and ready for close-up details.',
        imageUrl: '/assets/Back%20arch%20styling/3.jpg'
      },
      {
        title: 'Custom event mood',
        description: 'The arch can shift from playful to refined with palette choices, signage, floral accents, and the scale of the installation.',
        imageUrl: '/assets/Back%20arch%20styling/4.jpg'
      },
      {
        title: 'Package-ready presentation',
        description: 'A polished package view helps customers understand how back arch styling can come together as a complete decor moment.',
        imageUrl: '/assets/Back%20arch%20styling/Shenara%20Package.png'
      }
    ]
  }
};

const legalPages = {
  privacy: {
    title: 'Privacy Policy',
    description: 'How Shenara Event Decor collects, uses, and protects inquiry and booking information.',
    sections: [
      {
        heading: 'Information we collect',
        body: 'Shenara Event Decor collects the details you provide in inquiry and booking forms, including name, email address, phone number, event date, event type, and planning notes.'
      },
      {
        heading: 'How information is used',
        body: 'Information is used to respond to decor requests, prepare quotes, coordinate event services, and follow up about active bookings.'
      },
      {
        heading: 'Access and deletion requests',
        body: 'Customers may request access to or deletion of their stored inquiry information by contacting Shenara Event Decor through the website contact path.'
      }
    ]
  },
  accessibility: {
    title: 'Accessibility Statement',
    description: 'Accessibility commitment for the Shenara Event Decor website and admin experience.',
    sections: [
      {
        heading: 'Accessibility commitment',
        body: 'Shenara Event Decor is working toward an experience that supports keyboard navigation, clear structure, readable contrast, and accessible forms across public and admin surfaces.'
      },
      {
        heading: 'Need help or found an issue?',
        body: 'If you encounter an accessibility barrier, please use the inquiry form to report it and include the page, device, and issue details so it can be reviewed promptly.'
      }
    ]
  },
  terms: {
    title: 'Terms of Service',
    description: 'General service expectations for inquiries, quotes, and event decor coordination.',
    sections: [
      {
        heading: 'Quotes and availability',
        body: 'All service availability, pricing, and scope are subject to confirmation at the time of inquiry and may change based on event date, venue, setup complexity, and inventory availability.'
      },
      {
        heading: 'Custom work',
        body: 'Custom decor concepts may require additional planning, approvals, and lead time. Final styling details should be confirmed before the event date.'
      }
    ]
  }
};

const contactDetails = {
  businessName: 'Shenara Event Decor',
  location: 'Serving celebration bookings by appointment',
  support: 'Use the booking inquiry form for quotes, availability, and accessibility support.'
};

const emptyInventory = {
  name: '',
  description: '',
  inventoryCategoryId: 1,
  totalQuantity: 1,
  availableQuantity: 1,
  minimumStockAlert: 1,
  status: 'Active',
  primaryImageUrl: '',
  color: '',
  size: '',
  storageLocation: '',
  rentalPrice: '',
  isFeatured: false
};

const inventoryStatusOptions = ['Active', 'Inactive', 'Damaged', 'Unavailable'];
const inquiryStatusOptions = ['New', 'Contacted', 'Quoted', 'Booked', 'Completed', 'Cancelled'];

function slugify(value) {
  return value.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');
}

function servicePath(name) {
  return slugify(name) === 'custom-event-setups' ? '/contact' : `/services/${slugify(name)}`;
}

function galleryPath(image) {
  return `/gallery/${image.id}`;
}

function currentRoute() {
  return `${window.location.pathname}${window.location.hash}`;
}

function isInternalPath(href) {
  return href.startsWith('/') || href.startsWith('#');
}

function routeFromUrl(url) {
  return `${url.pathname}${url.hash}`;
}

function setPageMeta(title, description) {
  document.title = `${title} | Shenara Event Decor`;

  let descriptionTag = document.querySelector('meta[name="description"]');
  if (!descriptionTag) {
    descriptionTag = document.createElement('meta');
    descriptionTag.name = 'description';
    document.head.appendChild(descriptionTag);
  }
  descriptionTag.setAttribute('content', description);

  let ogTitle = document.querySelector('meta[property="og:title"]');
  if (!ogTitle) {
    ogTitle = document.createElement('meta');
    ogTitle.setAttribute('property', 'og:title');
    document.head.appendChild(ogTitle);
  }
  ogTitle.setAttribute('content', `${title} | Shenara Event Decor`);

  let ogDescription = document.querySelector('meta[property="og:description"]');
  if (!ogDescription) {
    ogDescription = document.createElement('meta');
    ogDescription.setAttribute('property', 'og:description');
    document.head.appendChild(ogDescription);
  }
  ogDescription.setAttribute('content', description);

  let canonicalTag = document.querySelector('link[rel="canonical"]');
  if (!canonicalTag) {
    canonicalTag = document.createElement('link');
    canonicalTag.setAttribute('rel', 'canonical');
    document.head.appendChild(canonicalTag);
  }
  canonicalTag.setAttribute('href', `${window.location.origin}${window.location.pathname}`);
}

function usePageMeta(title, description) {
  useEffect(() => {
    setPageMeta(title, description);
  }, [title, description]);
}

async function request(path, options = {}) {
  let response;

  try {
    const { headers: optionHeaders, ...restOptions } = options;
    response = await fetch(`${API_URL}${path}`, {
      ...restOptions,
      headers: { 'Content-Type': 'application/json', ...optionHeaders }
    });
  } catch (networkError) {
    const error = new Error('Could not reach the Shenara API.');
    error.status = 0;
    error.body = null;
    error.cause = networkError;
    throw error;
  }

  if (!response.ok) {
    let body = null;
    let text = '';

    try {
      body = await response.json();
    } catch {
      try {
        text = await response.text();
      } catch {
        text = '';
      }
    }

    const error = new Error(`Request failed: ${response.status}`);
    error.status = response.status;
    error.body = body;
    error.text = text;
    throw error;
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function getRequestErrorMessage(error, fallbackMessage) {
  if (error.status === 0) {
    return 'The app could not reach the Shenara API. Please make sure the backend is running on http://localhost:5189.';
  }

  if (error.body?.errors) {
    const firstError = Object.values(error.body.errors).flat()[0];
    if (firstError) {
      return firstError;
    }
  }

  if (typeof error.body?.message === 'string' && error.body.message) {
    return error.body.message;
  }

  if (typeof error.body?.title === 'string' && error.body.title) {
    return error.body.title;
  }

  if (typeof error.text === 'string' && error.text.trim()) {
    return error.text.trim();
  }

  return fallbackMessage;
}

function App() {
  const [route, setRoute] = useState(currentRoute());

  useEffect(() => {
    const onLocationChange = () => setRoute(currentRoute());
    const onDocumentClick = (event) => {
      const link = event.target.closest('a[href]');

      if (!link || link.target || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) {
        return;
      }

      const href = link.getAttribute('href');

      if (!href || !isInternalPath(href) || href.startsWith('mailto:')) {
        return;
      }

      const nextUrl = new URL(href, window.location.href);

      if (nextUrl.origin !== window.location.origin) {
        return;
      }

      event.preventDefault();
      window.history.pushState({}, '', `${nextUrl.pathname}${nextUrl.hash}`);
      setRoute(routeFromUrl(nextUrl));
    };

    window.addEventListener('popstate', onLocationChange);
    window.addEventListener('hashchange', onLocationChange);
    document.addEventListener('click', onDocumentClick);

    return () => {
      window.removeEventListener('popstate', onLocationChange);
      window.removeEventListener('hashchange', onLocationChange);
      document.removeEventListener('click', onDocumentClick);
    };
  }, []);

  useEffect(() => {
    if (route === '/services/custom-event-setups') {
      window.history.replaceState({}, '', '/contact');
      setRoute('/contact');
      return;
    }

    const [path, hash] = route.split('#');
    const sectionId = hash || (path === '/services' || path === '/gallery' || path === '/contact'
      ? path.replace('/', '')
      : '');

    if (!sectionId) {
      window.scrollTo({ top: 0, behavior: 'smooth' });
      return;
    }

    window.requestAnimationFrame(() => {
      document.getElementById(sectionId)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }, [route]);

  return route.startsWith('/admin') ? <AdminPortal route={route} /> : <PublicSite route={route} />;
}

function PublicSite({ route }) {
  const [services, setServices] = useState(fallbackServices);
  const [gallery, setGallery] = useState(galleryCollections);
  const [inquiry, setInquiry] = useState({ customerName: '', email: '', phone: '', eventDate: '', eventType: '', message: '' });
  const [inquiryFeedback, setInquiryFeedback] = useState({ type: '', message: '' });

  useEffect(() => {
    request('/services').then(setServices).catch(() => setServices(fallbackServices));
  }, []);

  const routeView = useMemo(() => {
    const [path] = route.split('#');

    if (path.startsWith('/services/')) {
      return { type: 'service', slug: path.replace('/services/', '') };
    }

    if (path.startsWith('/gallery/')) {
      return { type: 'gallery', slug: path.replace('/gallery/', '') };
    }

    if (path === '/privacy' || path === '/accessibility' || path === '/terms') {
      return { type: 'legal', key: path.replace('/', '') };
    }

    return { type: 'home' };
  }, [route]);

  async function submitInquiry(event) {
    event.preventDefault();
    setInquiryFeedback({ type: '', message: '' });

    try {
      await request('/inquiries', {
        method: 'POST',
        body: JSON.stringify({ ...inquiry, eventDate: inquiry.eventDate || null })
      });
      setInquiry({ customerName: '', email: '', phone: '', eventDate: '', eventType: '', message: '' });
      setInquiryFeedback({ type: 'success', message: 'Inquiry sent. Shenara Event Decor will be in touch soon.' });
    } catch (error) {
      setInquiryFeedback({ type: 'error', message: getRequestErrorMessage(error, 'Could not send your inquiry right now.') });
    }
  }

  if (routeView.type === 'service') {
    const service = services.find((entry) => slugify(entry.name) === routeView.slug) ?? fallbackServices.find((entry) => slugify(entry.name) === routeView.slug);
    return (
      <PublicShell>
        <ServiceDetailPage service={service} slug={routeView.slug} />
      </PublicShell>
    );
  }

  if (routeView.type === 'gallery') {
    const galleryItem =
      gallery.find((entry) => entry.id === routeView.slug) ??
      gallery.find((entry) => slugify(entry.title) === routeView.slug);
    return (
      <PublicShell>
        <GalleryDetailPage galleryItem={galleryItem} />
      </PublicShell>
    );
  }

  if (routeView.type === 'legal') {
    return (
      <PublicShell>
        <LegalPage pageKey={routeView.key} />
      </PublicShell>
    );
  }

  return (
    <PublicShell>
      <HomePage
        services={services}
        gallery={gallery}
        inquiry={inquiry}
        setInquiry={setInquiry}
        inquiryFeedback={inquiryFeedback}
        onSubmitInquiry={submitInquiry}
      />
    </PublicShell>
  );
}

function PublicShell({ children }) {
  return (
    <div className="site-shell">
      <a className="skip-link" href="#main-content">Skip to main content</a>
      <header className="topbar">
        <a className="brand brand-with-logo" href="/" aria-label="Shenara Event Decor home">
          <img className="brand-logo" src={LOGO_URL} alt="" />
          <span className="brand-text">Shenara <span>Event Decor</span></span>
        </a>
        <nav aria-label="Primary navigation">
          <a href="/services">Services</a>
          <a href="/gallery">Gallery</a>
          <a href="/contact">Contact</a>
          <a href="/privacy">Privacy</a>
        </nav>
      </header>
      <main id="main-content">
        {children}
      </main>
      <footer className="site-footer">
        <div>
          <strong>{contactDetails.businessName}</strong>
          <p>{contactDetails.location}</p>
          <p>{contactDetails.support}</p>
        </div>
        <nav aria-label="Footer navigation">
          <a href="/privacy">Privacy Policy</a>
          <a href="/accessibility">Accessibility Statement</a>
          <a href="/terms">Terms of Service</a>
          <a href="/contact">Contact</a>
        </nav>
      </footer>
    </div>
  );
}

function HomePage({ services, gallery, inquiry, setInquiry, inquiryFeedback, onSubmitInquiry }) {
  usePageMeta(
    'Shenara Event Decor',
    'Premium balloon decor, back arch styling, and custom event setups for birthdays, showers, and celebrations.'
  );

  return (
    <>
      <section className="hero" id="home">
        <div className="hero-copy">
          <p className="eyebrow">Balloon decor, arches, backdrops, and custom styling</p>
          <h1>Shenara Event Decor</h1>
          <p className="hero-text">Elegant event decor for celebrations that deserve a beautiful focal point, thoughtful colors, and a setup that feels made for the moment.</p>
          <div className="hero-actions">
            <a className="primary-action" href="/contact">Request a Quote</a>
            <a className="secondary-action" href="/gallery">View Gallery</a>
          </div>
        </div>
        <div className="hero-image">
          <img src="https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=1200&q=80" alt="Elegant balloon decor setup with layered celebration styling" />
        </div>
      </section>

      <section className="intro-band" aria-label="Service strengths">
        <div>
          <strong>Personalized styling</strong>
          <span>Color palettes, themes, signage, and venue-aware setup planning.</span>
        </div>
        <div>
          <strong>Decor inventory</strong>
          <span>Balloons, arches, drop cloths, props, and custom add-ons.</span>
        </div>
        <div>
          <strong>Event-ready finish</strong>
          <span>Designed for photos, entrances, dessert tables, and statement moments.</span>
        </div>
      </section>

      <section className="section" id="services">
        <div className="section-heading">
          <p className="eyebrow">Services</p>
          <h2>Decor that frames the celebration</h2>
        </div>
        <div className="service-grid">
          {services.map((service) => (
            <a className="service-card service-card-link" key={service.id} href={servicePath(service.name)}>
              <img src={service.imageUrl} alt={service.name} />
              <div>
                <span>{service.startingPrice}</span>
                <h3>{service.name}</h3>
                <p>{service.description}</p>
              </div>
            </a>
          ))}
        </div>
      </section>

      <section className="section gallery-section" id="gallery">
        <div className="section-heading">
          <p className="eyebrow">Gallery</p>
          <h2>Real setups for birthdays, showers, and special occasions</h2>
        </div>
        <div className="gallery-grid">
          {gallery.map((image) => (
            <a className="gallery-link" href={galleryPath(image)} key={image.id}>
              <figure>
                <img src={image.imageUrl} alt={image.title} />
                <figcaption>{image.title}<span>{image.summary}</span></figcaption>
              </figure>
            </a>
          ))}
        </div>
      </section>

      <section className="contact-section" id="contact">
        <div className="contact-copy">
          <p className="eyebrow">Booking Inquiry</p>
          <h2>Tell us what you are celebrating</h2>
          <p>Share your event date, theme, colors, and favorite decor ideas. Shenara Event Decor can shape the package around your moment.</p>
        </div>
        <form className="inquiry-form" onSubmit={onSubmitInquiry} noValidate>
          <FormField label="Name" inputId="inquiry-name">
            <input id="inquiry-name" required value={inquiry.customerName} onChange={(event) => setInquiry({ ...inquiry, customerName: event.target.value })} />
          </FormField>
          <FormField label="Email" inputId="inquiry-email">
            <input id="inquiry-email" required type="email" value={inquiry.email} onChange={(event) => setInquiry({ ...inquiry, email: event.target.value })} />
          </FormField>
          <FormField label="Phone" inputId="inquiry-phone">
            <input id="inquiry-phone" value={inquiry.phone} onChange={(event) => setInquiry({ ...inquiry, phone: event.target.value })} />
          </FormField>
          <FormField label="Event date" inputId="inquiry-date">
            <input id="inquiry-date" type="date" value={inquiry.eventDate} onChange={(event) => setInquiry({ ...inquiry, eventDate: event.target.value })} />
          </FormField>
          <FormField label="Event type" inputId="inquiry-type">
            <input id="inquiry-type" value={inquiry.eventType} onChange={(event) => setInquiry({ ...inquiry, eventType: event.target.value })} />
          </FormField>
          <FormField label="Tell us about your colors, venue, and decor ideas" inputId="inquiry-message">
            <textarea id="inquiry-message" required value={inquiry.message} onChange={(event) => setInquiry({ ...inquiry, message: event.target.value })} />
          </FormField>
          <p className="field-note">Required fields are marked by their labels and validation behavior, not color alone.</p>
          <button type="submit">Send Inquiry</button>
          <div aria-live="polite" className="feedback-region">
            {inquiryFeedback.message && (
              <p className={inquiryFeedback.type === 'error' ? 'error-note' : 'success-note'}>
                {inquiryFeedback.message}
              </p>
            )}
          </div>
        </form>
      </section>
    </>
  );
}

function ServiceDetailPage({ service, slug }) {
  const detail = serviceDetailContent[slug];

  if (!service || !detail) {
    usePageMeta('Service not found', 'The requested service detail page could not be found.');
    return (
      <section className="detail-shell">
        <div className="detail-header">
          <p className="eyebrow">Service Detail</p>
          <h1>Service not found</h1>
          <p className="hero-text">That service showcase is not available yet.</p>
          <a className="secondary-action" href="/services">Back to Services</a>
        </div>
      </section>
    );
  }

  usePageMeta(service.name, service.description);

  return (
    <section className="detail-shell">
      <div className="detail-header">
        <p className="eyebrow">{detail.eyebrow}</p>
        <h1>{detail.title}</h1>
        <p className="hero-text">{detail.intro}</p>
        <div className="hero-actions">
          <a className="secondary-action" href="/services">Back to Services</a>
          <a className="primary-action" href="/contact">Request This Style</a>
        </div>
      </div>
      <div className={`detail-grid detail-grid--${detail.highlights.length}`}>
        {detail.highlights.map((highlight) => (
          <article className="detail-card" key={highlight.title}>
            <img src={highlight.imageUrl} alt={highlight.title} />
            <div>
              <h2>{highlight.title}</h2>
              <p>{highlight.description}</p>
            </div>
          </article>
        ))}
      </div>
    </section>
  );
}

function GalleryDetailPage({ galleryItem }) {
  if (!galleryItem) {
    usePageMeta('Gallery item not found', 'The requested gallery detail page could not be found.');
    return (
      <section className="detail-shell">
        <div className="detail-header">
          <p className="eyebrow">Gallery</p>
          <h1>Gallery item not found</h1>
          <a className="secondary-action" href="/gallery">Back to Gallery</a>
        </div>
      </section>
    );
  }

  usePageMeta(galleryItem.title, `${galleryItem.title} from the Shenara Event Decor gallery.`);

  return (
    <section className="detail-shell">
      <div className="detail-header">
        <p className="eyebrow">Gallery</p>
        <h1>{galleryItem.title}</h1>
        <p className="hero-text">{galleryItem.summary}</p>
        <a className="secondary-action" href="/gallery">Back to Gallery</a>
      </div>
      <div className={`gallery-detail-grid gallery-detail-grid--${galleryItem.items.length}`}>
        {galleryItem.items.map((item) => (
          <article className="gallery-detail-card" key={item.mediaUrl}>
            {item.type === 'video' ? (
              <video src={item.mediaUrl} controls muted playsInline preload="metadata" aria-label={item.title} />
            ) : (
              <img src={item.mediaUrl} alt={item.title} />
            )}
            <div>
              <h2>{item.title}</h2>
              <p>{item.description}</p>
            </div>
          </article>
        ))}
      </div>
    </section>
  );
}

function LegalPage({ pageKey }) {
  const page = legalPages[pageKey] ?? legalPages.privacy;
  usePageMeta(page.title, page.description);

  return (
    <section className="detail-shell legal-shell">
      <div className="detail-header">
        <p className="eyebrow">Information</p>
        <h1>{page.title}</h1>
        <p className="hero-text">{page.description}</p>
      </div>
      <div className="legal-grid">
        {page.sections.map((section) => (
          <article className="legal-card" key={section.heading}>
            <h2>{section.heading}</h2>
            <p>{section.body}</p>
          </article>
        ))}
      </div>
    </section>
  );
}

function FormField({ label, inputId, children }) {
  return (
    <div className="form-field">
      <label htmlFor={inputId}>{label}</label>
      {children}
    </div>
  );
}

function AdminPortal({ route }) {
  const [token, setToken] = useState(sessionStorage.getItem(ADMIN_TOKEN_KEY) || '');

  if (!token) {
    return <LoginScreen onLogin={setToken} />;
  }

  return <AdminDashboard route={route} token={token} onLogout={() => { sessionStorage.removeItem(ADMIN_TOKEN_KEY); setToken(''); }} />;
}

function LoginScreen({ onLogin }) {
  usePageMeta('Admin Portal', 'Protected admin access for Shenara Event Decor inventory and inquiries.');

  const [credentials, setCredentials] = useState({ username: 'admin', password: 'admin123' });
  const [error, setError] = useState('');
  const [lockoutMessage, setLockoutMessage] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function login(event) {
    event.preventDefault();
    setError('');
    setLockoutMessage('');
    setIsSubmitting(true);
    try {
      const result = await request('/auth/login', { method: 'POST', body: JSON.stringify(credentials) });
      sessionStorage.setItem(ADMIN_TOKEN_KEY, result.token);
      onLogin(result.token);
    } catch (error) {
      if (error.status === 429) {
        const lockedUntil = error.body?.lockedUntilUtc ? new Date(error.body.lockedUntilUtc).toLocaleTimeString() : null;
        setLockoutMessage(lockedUntil
          ? `Too many failed attempts. Admin login is locked until ${lockedUntil}.`
          : 'Too many failed attempts. Admin login is temporarily locked.');
      } else if (error.status === 401) {
        setError(`Could not sign in. ${error.body?.remainingAttempts ?? 0} attempt(s) remaining.`);
      } else {
        setError(getRequestErrorMessage(error, 'Could not sign in with those credentials.'));
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="admin-login">
      <a className="skip-link" href="#admin-login-panel">Skip to main content</a>
      <form className="login-panel" onSubmit={login} id="admin-login-panel" noValidate>
        <a className="brand brand-with-logo" href="/" aria-label="Shenara Event Decor home">
          <img className="brand-logo" src={LOGO_URL} alt="" />
          <span className="brand-text">Shenara <span>Event Decor</span></span>
        </a>
        <h1>Admin Portal</h1>
        <p className="login-copy">Protected access for inventory, inquiries, and booking operations.</p>
        <FormField label="Username" inputId="admin-username">
          <input id="admin-username" value={credentials.username} onChange={(event) => setCredentials({ ...credentials, username: event.target.value })} />
        </FormField>
        <FormField label="Password" inputId="admin-password">
          <input id="admin-password" type="password" value={credentials.password} onChange={(event) => setCredentials({ ...credentials, password: event.target.value })} />
        </FormField>
        <button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Checking Access' : 'Sign In'}</button>
        <div aria-live="polite" className="feedback-region">
          {error && <p className="error-note">{error}</p>}
          {lockoutMessage && <p className="error-note">{lockoutMessage}</p>}
        </div>
      </form>
    </div>
  );
}

function AdminDashboard({ route, token, onLogout }) {
  usePageMeta('Admin Dashboard', 'Inventory and inquiry operations for Shenara Event Decor.');

  const [inventoryPage, setInventoryPage] = useState({
    items: [],
    page: 1,
    pageSize: 12,
    totalCount: 0,
    totalPages: 1,
    lowStockCount: 0,
    featuredCount: 0
  });
  const [inquiries, setInquiries] = useState([]);
  const [inquiriesError, setInquiriesError] = useState('');
  const [lastViewed, setLastViewed] = useState(sessionStorage.getItem(INQUIRIES_VIEWED_KEY) || '');
  const activeView = route.startsWith('/admin/inquiries') ? 'inquiries' : 'inventory';

  const newInquiryCount = useMemo(() => {
    if (!lastViewed) {
      return inquiries.length;
    }

    return inquiries.filter((inquiry) => new Date(inquiry.createdAt) > new Date(lastViewed)).length;
  }, [inquiries, lastViewed]);

  const newStatusCount = inquiries.filter((inquiry) => inquiry.status === 'New').length;

  useEffect(() => {
    refreshItems();
    refreshInquiries();
  }, []);

  useEffect(() => {
    if (activeView === 'inquiries') {
      const viewedAt = new Date().toISOString();
      sessionStorage.setItem(INQUIRIES_VIEWED_KEY, viewedAt);
      setLastViewed(viewedAt);
    }
  }, [activeView, inquiries.length]);

  function handleUnauthorized() {
    onLogout();
  }

  async function refreshItems() {
    request('/inventory?page=1&pageSize=12')
      .then(setInventoryPage)
      .catch(() => setInventoryPage({
        items: [],
        page: 1,
        pageSize: 12,
        totalCount: 0,
        totalPages: 1,
        lowStockCount: 0,
        featuredCount: 0
      }));
  }

  async function refreshInquiries() {
    setInquiriesError('');

    request('/inquiries', { headers: { Authorization: `Bearer ${token}` } })
      .then((result) => {
        setInquiries(result);
        setInquiriesError('');
      })
      .catch((error) => {
        if (error.status === 401) {
          handleUnauthorized();
        } else {
          setInquiries([]);
          setInquiriesError(getRequestErrorMessage(error, 'Could not load booking inquiries right now.'));
        }
      });
  }

  async function updateInquiryStatus(id, nextStatus) {
    const updated = await request(`/inquiries/${id}/status`, {
      method: 'PATCH',
      headers: { Authorization: `Bearer ${token}` },
      body: JSON.stringify({ status: nextStatus })
    });

    setInquiries((current) => current.map((inquiry) => inquiry.id === id ? updated : inquiry));
  }

  return (
    <div className="admin-shell">
      <aside className="admin-sidebar">
        <a className="brand brand-with-logo" href="/" aria-label="Shenara Event Decor home">
          <img className="brand-logo" src={LOGO_URL} alt="" />
          <span className="brand-text">Shenara <span>Event Decor</span></span>
        </a>
        <nav aria-label="Admin navigation">
          <a className={activeView === 'inventory' ? 'active' : ''} href="/admin">Inventory</a>
          <a className={activeView === 'inquiries' ? 'active with-badge' : 'with-badge'} href="/admin/inquiries">
            Inquiries
            {newInquiryCount > 0 && <span className="nav-badge">{newInquiryCount}</span>}
          </a>
          <a href="/">Public Site</a>
        </nav>
      </aside>

      <main className="admin-main">
        <div className="admin-header">
          <div>
            <p className="eyebrow">Admin Portal</p>
            <h1>{activeView === 'inquiries' ? 'Booking Inquiries' : 'Inventory Management'}</h1>
          </div>
          <div className="admin-header-actions">
            <div className="metric-row">
              {activeView === 'inquiries' ? (
                <>
                  <Metric label="Total" value={inquiries.length} />
                  <Metric label="New Status" value={newStatusCount} />
                  <Metric label="Since Viewed" value={newInquiryCount} />
                </>
              ) : (
                <>
                  <Metric label="Items" value={inventoryPage.totalCount} />
                  <Metric label="Low Stock" value={inventoryPage.lowStockCount} />
                  <Metric label="Featured" value={inventoryPage.featuredCount} />
                </>
              )}
            </div>
            <button className="quiet-button top-signout" type="button" onClick={onLogout}>Sign Out</button>
          </div>
        </div>

        {activeView === 'inquiries' ? (
          <InquiriesView inquiries={inquiries} loadError={inquiriesError} onStatusChange={updateInquiryStatus} />
        ) : (
          <InventoryView token={token} inventoryPage={inventoryPage} setInventoryPage={setInventoryPage} onUnauthorized={handleUnauthorized} />
        )}
      </main>
    </div>
  );
}

function InventoryView({ token, inventoryPage, setInventoryPage, onUnauthorized }) {
  const [categories, setCategories] = useState([]);
  const [colors, setColors] = useState([]);
  const [form, setForm] = useState(emptyInventory);
  const [editingId, setEditingId] = useState(null);
  const [editingLabel, setEditingLabel] = useState('');
  const [query, setQuery] = useState('');
  const [status, setStatus] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('');
  const [colorFilter, setColorFilter] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [isSaving, setIsSaving] = useState(false);
  const [isLoadingEdit, setIsLoadingEdit] = useState(false);
  const formRef = useRef(null);
  const nameInputRef = useRef(null);

  useEffect(() => {
    request('/inventory/categories').then(setCategories).catch(() => setCategories([]));
    request('/inventory/colors').then(setColors).catch(() => setColors([]));
  }, []);

  useEffect(() => {
    refreshInventory(1);
  }, [query, status, categoryFilter, colorFilter]);

  async function refreshInventory(nextPage = inventoryPage.page) {
    const params = new URLSearchParams({
      page: String(nextPage),
      pageSize: String(inventoryPage.pageSize)
    });

    if (query) params.set('search', query);
    if (status) params.set('status', status);
    if (categoryFilter) params.set('categoryId', categoryFilter);
    if (colorFilter) params.set('color', colorFilter);

    const result = await request(`/inventory?${params.toString()}`);
    setInventoryPage(result);
  }

  function setEditForm(item) {
    setForm({
      name: item.name,
      description: item.description || '',
      inventoryCategoryId: item.inventoryCategoryId,
      totalQuantity: item.totalQuantity,
      availableQuantity: item.availableQuantity,
      minimumStockAlert: item.minimumStockAlert,
      status: item.status,
      primaryImageUrl: item.primaryImageUrl || '',
      color: item.color || '',
      size: item.size || '',
      storageLocation: item.storageLocation || '',
      rentalPrice: item.rentalPrice ?? '',
      isFeatured: item.isFeatured
    });
  }

  async function edit(item) {
    setError('');
    setMessage('');
    setIsLoadingEdit(true);

    try {
      const fullItem = await request(`/inventory/${item.id}`);
      setEditingId(fullItem.id);
      setEditingLabel(fullItem.name);
      setEditForm(fullItem);
    } catch {
      setEditingId(item.id);
      setEditingLabel(item.name);
      setEditForm(item);
    } finally {
      setIsLoadingEdit(false);
      formRef.current?.scrollIntoView({ behavior: 'smooth', block: 'start' });
      window.setTimeout(() => nameInputRef.current?.focus(), 120);
    }
  }

  async function save(event) {
    event.preventDefault();
    setError('');
    setMessage('');
    setIsSaving(true);

    const payload = {
      ...form,
      inventoryCategoryId: Number(form.inventoryCategoryId),
      totalQuantity: Number(form.totalQuantity),
      availableQuantity: Number(form.availableQuantity),
      minimumStockAlert: Number(form.minimumStockAlert),
      rentalPrice: form.rentalPrice === '' ? null : Number(form.rentalPrice)
    };

    const currentEditingId = editingId;
    const path = currentEditingId ? `/inventory/${currentEditingId}` : '/inventory';

    try {
      const savedItem = await request(path, {
        method: currentEditingId ? 'PUT' : 'POST',
        headers: { Authorization: `Bearer ${token}` },
        body: JSON.stringify(payload)
      });
      if (currentEditingId) {
        setInventoryPage((currentPage) => ({
          ...currentPage,
          items: currentPage.items.map((item) => item.id === savedItem.id ? savedItem : item)
        }));
      }
      setForm(emptyInventory);
      setEditingId(null);
      setEditingLabel('');
      setMessage(currentEditingId ? 'Inventory item updated successfully.' : 'Inventory item added successfully.');
      await refreshInventory(currentEditingId ? inventoryPage.page : 1);
    } catch (requestError) {
      if (requestError.status === 401 || requestError.status === 403) {
        setError('Your admin session has expired. Please sign in again.');
        onUnauthorized();
      } else {
        setError(getRequestErrorMessage(requestError, 'Could not save this inventory item right now.'));
      }
    } finally {
      setIsSaving(false);
    }
  }

  async function remove(id) {
    if (!window.confirm('Delete this inventory item?')) {
      return;
    }

    try {
      await request(`/inventory/${id}`, { method: 'DELETE', headers: { Authorization: `Bearer ${token}` } });
      setMessage('Inventory item deleted.');
      await refreshInventory(inventoryPage.page);
    } catch (requestError) {
      if (requestError.status === 401 || requestError.status === 403) {
        onUnauthorized();
      } else {
        setError(getRequestErrorMessage(requestError, 'Could not delete this inventory item right now.'));
      }
    }
  }

  return (
    <section className="admin-grid">
      <form className="inventory-form" onSubmit={save} ref={formRef} noValidate>
        <h2>{editingId ? 'Edit Inventory' : 'Add Inventory'}</h2>
        {editingId && (
          <div className="edit-banner">
            <strong>Editing item</strong>
            <span>{editingLabel}</span>
          </div>
        )}
        <FormField label="Item name" inputId="inventory-name">
          <input ref={nameInputRef} id="inventory-name" required value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
        </FormField>
        <FormField label="Category" inputId="inventory-category">
          <select id="inventory-category" value={form.inventoryCategoryId} onChange={(event) => setForm({ ...form, inventoryCategoryId: event.target.value })}>
            {categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}
          </select>
        </FormField>
        <FormField label="Description" inputId="inventory-description">
          <textarea id="inventory-description" value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} />
        </FormField>
        <div className="form-pair">
          <FormField label="Total quantity" inputId="inventory-total-qty">
            <input id="inventory-total-qty" type="number" min="0" value={form.totalQuantity} onChange={(event) => setForm({ ...form, totalQuantity: event.target.value })} />
          </FormField>
          <FormField label="Available quantity" inputId="inventory-available-qty">
            <input id="inventory-available-qty" type="number" min="0" value={form.availableQuantity} onChange={(event) => setForm({ ...form, availableQuantity: event.target.value })} />
          </FormField>
        </div>
        <div className="form-pair">
          <FormField label="Low stock alert" inputId="inventory-low-stock">
            <input id="inventory-low-stock" type="number" min="0" value={form.minimumStockAlert} onChange={(event) => setForm({ ...form, minimumStockAlert: event.target.value })} />
          </FormField>
          <FormField label="Status" inputId="inventory-status">
            <select id="inventory-status" value={form.status} onChange={(event) => setForm({ ...form, status: event.target.value })}>
              {inventoryStatusOptions.map((option) => <option key={option} value={option}>{option}</option>)}
            </select>
          </FormField>
        </div>
        <FormField label="Image URL" inputId="inventory-image-url">
          <input id="inventory-image-url" value={form.primaryImageUrl} onChange={(event) => setForm({ ...form, primaryImageUrl: event.target.value })} />
        </FormField>
        <div className="form-pair">
          <FormField label="Color" inputId="inventory-color">
            <input id="inventory-color" value={form.color} onChange={(event) => setForm({ ...form, color: event.target.value })} />
          </FormField>
          <FormField label="Size" inputId="inventory-size">
            <input id="inventory-size" value={form.size} onChange={(event) => setForm({ ...form, size: event.target.value })} />
          </FormField>
        </div>
        <div className="form-pair">
          <FormField label="Storage location" inputId="inventory-storage">
            <input id="inventory-storage" value={form.storageLocation} onChange={(event) => setForm({ ...form, storageLocation: event.target.value })} />
          </FormField>
          <FormField label="Rental price" inputId="inventory-rental-price">
            <input id="inventory-rental-price" type="number" min="0" step="0.01" value={form.rentalPrice} onChange={(event) => setForm({ ...form, rentalPrice: event.target.value })} />
          </FormField>
        </div>
        <label className="check-row" htmlFor="inventory-featured">
          <input id="inventory-featured" type="checkbox" checked={form.isFeatured} onChange={(event) => setForm({ ...form, isFeatured: event.target.checked })} />
          Featured public item
        </label>
        <button type="submit" disabled={isSaving || isLoadingEdit}>{isSaving ? 'Saving Changes' : editingId ? 'Save Changes' : 'Add Item'}</button>
        {editingId && <button className="quiet-button" type="button" onClick={() => { setEditingId(null); setEditingLabel(''); setForm(emptyInventory); setError(''); setMessage('Edit canceled.'); }}>Cancel Edit</button>}
        <div aria-live="polite" className="feedback-region">
          {message && <p className="success-note">{message}</p>}
          {error && <p className="error-note">{error}</p>}
        </div>
      </form>

      <section className="inventory-panel">
        <div className="inventory-panel-header">
          <div className="panel-title">
            <h2>Inventory Directory</h2>
            <p>Browse the full catalog with server-backed paging so larger inventory sets stay fast and tidy.</p>
          </div>
        </div>
        <div className="inventory-tools">
          <FormField label="Search inventory" inputId="inventory-filter-search">
            <input id="inventory-filter-search" value={query} onChange={(event) => setQuery(event.target.value)} />
          </FormField>
          <FormField label="Category filter" inputId="inventory-filter-category">
            <select id="inventory-filter-category" value={categoryFilter} onChange={(event) => setCategoryFilter(event.target.value)}>
              <option value="">All categories</option>
              {categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}
            </select>
          </FormField>
          <FormField label="Color filter" inputId="inventory-filter-color">
            <select id="inventory-filter-color" value={colorFilter} onChange={(event) => setColorFilter(event.target.value)}>
              <option value="">All colors</option>
              {colors.map((color) => <option key={color} value={color}>{color}</option>)}
            </select>
          </FormField>
          <FormField label="Status filter" inputId="inventory-filter-status">
            <select id="inventory-filter-status" value={status} onChange={(event) => setStatus(event.target.value)}>
              <option value="">All statuses</option>
              {inventoryStatusOptions.map((option) => <option key={option} value={option}>{option}</option>)}
            </select>
          </FormField>
        </div>
        <div className="inventory-table">
          {inventoryPage.items.map((item) => (
            <article className="inventory-row" key={item.id}>
              <img src={item.primaryImageUrl || 'https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=300&q=80'} alt={item.name} />
              <div>
                <h3>{item.name}</h3>
                <p>{item.categoryName} - {item.color || 'Any color'} - {item.size || 'Flexible'}</p>
              </div>
              <span className={item.availableQuantity <= item.minimumStockAlert ? 'stock-badge low' : 'stock-badge'}>{item.availableQuantity}/{item.totalQuantity}</span>
              <span className="status-badge">{item.status}</span>
              <div className="row-actions">
                <button type="button" disabled={isLoadingEdit} onClick={() => edit(item)}>{isLoadingEdit && editingId === item.id ? 'Loading' : 'Edit'}</button>
                <button type="button" className="danger" onClick={() => remove(item.id)}>Delete</button>
              </div>
            </article>
          ))}
          {!inventoryPage.items.length && <div className="empty-state">No inventory items match this view.</div>}
        </div>
        <div className="pagination-bar">
          <p>Showing page {inventoryPage.page} of {inventoryPage.totalPages} - {inventoryPage.totalCount} item(s)</p>
          <div className="pagination-actions">
            <button type="button" className="quiet-button" disabled={inventoryPage.page <= 1} onClick={() => refreshInventory(inventoryPage.page - 1)}>Previous</button>
            <button type="button" className="quiet-button" disabled={inventoryPage.page >= inventoryPage.totalPages} onClick={() => refreshInventory(inventoryPage.page + 1)}>Next</button>
          </div>
        </div>
      </section>
    </section>
  );
}

function InquiriesView({ inquiries, loadError, onStatusChange }) {
  const [status, setStatus] = useState('');
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');

  const visibleInquiries = useMemo(() => {
    return inquiries.filter((inquiry) => !status || inquiry.status === status);
  }, [inquiries, status]);

  async function updateStatus(id, nextStatus) {
    setMessage('');
    setError('');

    try {
      await onStatusChange(id, nextStatus);
      setMessage('Inquiry status updated.');
    } catch (requestError) {
      setError(getRequestErrorMessage(requestError, 'Could not update inquiry status right now.'));
    }
  }

  return (
    <section className="inquiries-panel">
      <div className="inventory-tools inquiries-toolbar">
        <div className="panel-title">
          <h2>Customer Requests</h2>
          <p>Review event details, contact customers, and move each inquiry through the booking workflow.</p>
        </div>
        <FormField label="Inquiry status filter" inputId="inquiry-status-filter">
          <select id="inquiry-status-filter" value={status} onChange={(event) => setStatus(event.target.value)}>
            <option value="">All statuses</option>
            {inquiryStatusOptions.map((option) => <option key={option} value={option}>{option}</option>)}
          </select>
        </FormField>
      </div>

      <div aria-live="polite" className="feedback-region">
        {loadError && <p className="error-note">{loadError}</p>}
        {message && <p className="success-note">{message}</p>}
        {error && <p className="error-note">{error}</p>}
      </div>

      <div className="inquiry-list">
        {visibleInquiries.map((inquiry) => (
          <article className={inquiry.status === 'New' ? 'inquiry-card is-new' : 'inquiry-card'} key={inquiry.id}>
            <div className="inquiry-main">
              <div>
                <div className="inquiry-topline">
                  <h3>{inquiry.customerName}</h3>
                  <span className="status-badge">{inquiry.status}</span>
                </div>
                <p>{inquiry.message}</p>
              </div>
              <dl className="inquiry-details">
                <div>
                  <dt>Email</dt>
                  <dd><a href={`mailto:${inquiry.email}`}>{inquiry.email}</a></dd>
                </div>
                <div>
                  <dt>Phone</dt>
                  <dd>{inquiry.phone || 'Not provided'}</dd>
                </div>
                <div>
                  <dt>Event</dt>
                  <dd>{inquiry.eventType || 'Not specified'}</dd>
                </div>
                <div>
                  <dt>Date</dt>
                  <dd>{inquiry.eventDate || 'Flexible'}</dd>
                </div>
                <div>
                  <dt>Received</dt>
                  <dd>{new Date(inquiry.createdAt).toLocaleString()}</dd>
                </div>
              </dl>
            </div>
            <div className="inquiry-actions">
              <FormField label={`Update status for ${inquiry.customerName}`} inputId={`inquiry-status-${inquiry.id}`}>
                <select id={`inquiry-status-${inquiry.id}`} value={inquiry.status} onChange={(event) => updateStatus(inquiry.id, event.target.value)}>
                  {inquiryStatusOptions.map((option) => <option key={option} value={option}>{option}</option>)}
                </select>
              </FormField>
              <a className="secondary-action" href={`mailto:${inquiry.email}`}>Email</a>
            </div>
          </article>
        ))}
        {!visibleInquiries.length && <div className="empty-state">No booking inquiries match this view.</div>}
      </div>
    </section>
  );
}

function Metric({ label, value }) {
  return (
    <div className="metric">
      <strong>{value}</strong>
      <span>{label}</span>
    </div>
  );
}

createRoot(document.getElementById('root')).render(<App />);

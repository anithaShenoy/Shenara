# Shenara Event Decor - Implementation Plan

## Project Snapshot

Shenara Event Decor is now structured as a two-part experience:

1. A polished public-facing React website for customers exploring decor services.
2. A protected admin portal for managing inventory and booking inquiries through the ASP.NET Core API and SQL Server database.

This plan reflects the current product direction and the design changes already implemented in the app.

## Current Design Direction

### Public Website

The public website uses a premium event-brand visual language instead of a generic SaaS layout.

Implemented design choices:

- Warm neutral background with rose, clay, sage, and gold accent tones
- Large editorial hero with strong event imagery
- Elegant serif-forward heading treatment for brand presence
- Service cards with photography and simple pricing cues
- Gallery section with image-led composition
- Inquiry section designed as a clean booking form rather than a plain contact block

Public UX goals:

- Make Shenara Event Decor feel warm, polished, and trustworthy
- Lead with actual decor categories and event styling examples
- Keep calls to action visible and simple
- Preserve strong mobile readability and layout stability

### Admin Portal

The admin portal has been redesigned to feel operational, organized, and professional.

Implemented design and workflow changes:

- Sidebar-based admin layout
- Top header area with visible `Sign Out`
- Dashboard metrics for inventory and inquiries
- Cleaner inventory management form with edit state support
- Server-backed paging for large inventory lists
- Filters for search, category, color, and status
- Improved row layout with thumbnails, quantity badges, and status badges
- Inquiry area with new-item badge and inquiry status management

Admin UX goals:

- Support repeated daily use without clutter
- Make stock state and edit actions easy to scan
- Keep the UI responsive across laptop and mobile widths
- Show meaningful success, error, loading, and empty states

## Implemented Functional Changes

### Inventory Management

Completed:

- Create inventory items
- Read inventory items
- Edit inventory items
- Delete inventory items
- Category filtering
- Color filtering
- Status filtering
- Search filtering
- Server-backed pagination
- Distinct placeholder images inferred from inventory descriptions
- Quantity tracking with low-stock indicators
- Featured item flag

Important edit-flow fixes completed:

- Edit mode now loads the latest item data before saving
- Edit mode clearly identifies the selected item
- Save now updates the visible list immediately
- Request handling preserves `Content-Type: application/json`
- Error handling now surfaces useful API messages instead of only generic failures

### Booking Inquiries

Completed:

- Public inquiry form posting to the API
- Admin inquiries screen
- Inquiry status updates
- New inquiries badge based on last viewed time
- Inquiry metrics for total, new status, and since viewed

### Admin Authentication and Security

Completed:

- Admin login screen
- Token-protected admin API routes
- Fixed-time credential comparison
- In-memory login attempt tracking
- Lockout after 3 failed attempts
- Lockout messaging in the UI
- Better expired-session and authorization error feedback

Current note:

- The lockout state is currently in memory, so it resets when the API restarts.

## Backend State

Current API areas in place:

- `POST /api/auth/login`
- `GET /api/services`
- `GET /api/gallery`
- `POST /api/inquiries`
- `GET /api/inquiries`
- `PATCH /api/inquiries/{id}/status`
- `GET /api/inventory`
- `GET /api/inventory/{id}`
- `GET /api/inventory/categories`
- `GET /api/inventory/colors`
- `POST /api/inventory`
- `PUT /api/inventory/{id}`
- `DELETE /api/inventory/{id}`

Current backend implementation notes:

- SQL Server is the source of truth
- Entity Framework Core is used for data access
- DTOs are used for inventory and public content flows
- Inventory queries support paging and filter parameters
- Development CORS is configured for the React app

## Frontend State

Current frontend behaviors in place:

- Hash-based route split between public site and admin portal
- Shared API request helper
- Admin login persisted through local storage token
- Inventory form with create/edit modes
- Inventory table with thumbnails and actions
- Inquiry management view
- Professional styling with shared CSS variables

## Design Standards Going Forward

Continue using these standards:

- Keep the public experience image-led and brand-forward
- Keep the admin experience quieter and operational
- Prefer compact, polished controls over oversized marketing UI in admin
- Preserve the current palette and spacing rhythm unless there is a clear design reason to change it
- Keep mobile behavior stable and avoid text overlap or broken grid states

## Accessibility Requirements

All future UI work for Shenara Event Decor should follow these accessibility requirements.

### 1. Images and Media

- All informative images must include meaningful `alt=""` text
- Decorative images must use empty alt text: `alt=""`
- `<img>` elements must not use `role="presentation"` unless that is genuinely appropriate
- `<picture>` usage must still provide alt text on the fallback `<img>`
- Videos must include captions with `<track kind="captions">`
- Audio and video content must have transcripts available
- No autoplaying audio without user-initiated control

### 2. Forms

- Every input must have a programmatically associated `<label for="">`
- Placeholder text must not be used as the only label
- Inputs must have unique `id` attributes per page
- Required fields must be indicated with text, not color alone
- Error messages must be tied to fields using `aria-describedby`
- Validation errors must be announced via `aria-live="polite"`

### 3. Keyboard Support

- All interactive elements must be reachable via `Tab`
- Focus order must match DOM order
- No custom components without keyboard support
- Focus must remain visible and outlines must not be removed without an equivalent replacement
- No keyboard traps; `Esc` or `Tab` must allow exit from modal contexts as appropriate
- Modals must trap focus internally while open

### 4. Color and Contrast

- Text contrast must be at least 4.5:1
- Large text contrast must be at least 3:1
- UI components such as buttons, inputs, and links must meet at least 3:1 contrast
- Hover and focus states must include a visible contrast change
- No color-only indicators; use icons, text, patterns, or another non-color cue

### 5. Semantic Structure

- One `<h1>` per page
- Heading levels must not be skipped unnecessarily
- Lists must use semantic elements like `<ul>`, `<ol>`, and `<dl>`
- Buttons must use `<button>`, not clickable `<div>` or `<span>`
- Links must use `<a href="">` with descriptive text
- Landmarks must be used appropriately:
  - `<header>`
  - `<nav>`
  - `<main>`
  - `<footer>`
  - `<aside>`

### 6. Navigation

- Each page must have a unique and descriptive `<title>`
- A "Skip to main content" link must be present and visible on focus
- Navigation order must stay consistent across pages
- Breadcrumbs, when used, must use semantic markup such as `<nav aria-label="breadcrumb">`
- Link text must describe its destination and should not use vague phrases like "Click here"

### 7. Responsive and Reflow

- Layout must work at 320px width
- No horizontal scrolling except where genuinely required, such as data tables
- Content must reflow correctly at 400% zoom
- Touch targets must be at least 44px by 44px
- No content should be hidden behind fixed headers on mobile

### 8. ARIA and Custom Components

- ARIA should only be used when native HTML cannot achieve the needed behavior
- Custom widgets must include:
  - keyboard support for relevant keys such as `Tab`, `Enter`, `Space`, and arrow keys
  - correct roles such as `tablist`, `dialog`, or `accordion`
  - state attributes such as `aria-expanded` and `aria-selected`
  - relationships such as `aria-controls` and `aria-labelledby`
- Live regions must be used for async updates where appropriate, typically `aria-live="polite"`

### 9. Performance and Readability

- Line height must be at least 1.5
- Paragraph spacing must be at least 1.5 times the line height
- No flashing content above 3 flashes per second
- Avoid long unbroken text blocks
- Body text size should be at least 16px

### 10. Legal and Third-Party

- Publish an accessibility statement
- Provide a contact method for reporting accessibility issues
- Test any third-party widgets such as booking, chat, or payment tools for accessibility
- PDFs must be accessible or replaced with HTML equivalents where possible

### 11. Developer Tooling

- Run automated checks using tools such as axe, Lighthouse, or Pa11y
- Include accessibility tests in CI where practical
- Use Storybook accessibility tooling for component-level checks if Storybook is introduced
- Use ESLint plugins such as:
  - `eslint-plugin-jsx-a11y`
  - `eslint-plugin-react`
- Use TypeScript if the frontend is later upgraded and stronger DOM or ARIA typing becomes important

### Accessibility Review Workflow

For future frontend work, include accessibility review in the normal done criteria:

- Check semantic HTML first before adding ARIA
- Verify keyboard navigation and visible focus states
- Verify labels, validation, and error announcements on forms
- Check contrast and responsive behavior on mobile and zoomed layouts
- Run at least one automated accessibility pass before signoff

### WCAG 2.1 AA Baseline

The product should meet WCAG 2.1 AA success criteria as the default accessibility target.

Minimum expectations:

- Alt text for all informative images
- Keyboard-only navigation support
- Color contrast of at least 4.5:1 for standard text
- Semantic HTML structure
- Accessible forms with labels and error states
- Captions for videos and transcripts for audio
- ARIA only when needed
- "Skip to content" link
- Accessible modals, menus, and accordions
- No flashing content above 3 flashes per second

## Privacy and Data Protection Requirements

Shenara Event Decor should treat customer inquiry and booking data as protected business data and follow Canada-focused privacy expectations alongside general web privacy good practice.

### PIPEDA and Privacy Requirements

- Publish a Privacy Policy and keep it accessible from the site
- Clearly explain what personal data is collected
- Obtain consent for personal data collection
- Allow users to request access to or deletion of their data
- Disclose third-party services used for analytics, booking, or payments

### Cookies and Tracking

- Show a cookie banner if tracking or analytics is used
- Allow users to opt out of non-essential cookies
- If Google Analytics is used, configure it with IP anonymization or the closest current privacy-safe equivalent

### Forms and Data Handling

- Contact and booking forms must use HTTPS
- Data must be stored securely in transit and at rest
- Do not collect unnecessary personal data

## Security Compliance Requirements

The project should follow secure-by-default web application practices.

### Transport and Session Security

- Enforce HTTPS and enable HSTS
- Use TLS 1.2 or newer
- Do not allow mixed content on HTTPS pages
- Use secure cookies with `HttpOnly`, `SameSite`, and `Secure` where cookies are used

### Application Security

- Add rate limiting on public forms to reduce spam and abuse
- Use reCAPTCHA or hCaptcha on public forms when moving toward production
- Provide CSRF protection on all relevant POST requests
- Protect against XSS by escaping user input and using a Content Security Policy where appropriate
- Keep dependencies up to date across npm, .NET, and any other package ecosystems
- Do not expose admin panels publicly without protection
- Do not use default credentials in production

## Business and Legal Requirements

### Required Pages

The public website should include or plan for these pages as applicable:

- Privacy Policy
- Terms of Service if services or products are being sold formally
- Refund or Return Policy if e-commerce is introduced
- Accessibility Statement
- Contact page with valid business information

### E-commerce Requirements

If the site later supports direct payments or online purchases:

- Tax calculation must be compliant with Canadian provincial requirements
- Payment processing must use a secure provider such as Stripe or Square
- Payment flow must be PCI-compliant and must never store raw card data in the app database
- Order confirmation emails should be sent automatically

### Email and Marketing Compliance

If marketing emails are introduced, comply with CASL:

- Obtain explicit consent for marketing emails
- Include an unsubscribe link in every marketing email
- Include a business address in the email footer

## SEO and Technical Standards

The public website should be discoverable, structured, and technically clean.

- Unique `<title>` and `<meta description>` per page
- Open Graph tags for social sharing
- XML sitemap available and submitted to search engines when live
- `robots.txt` configured
- Canonical URLs used to prevent duplicate indexing
- Structured data using JSON-LD for business information
- Performance targets should aim for strong Core Web Vitals, including LCP under 2.5 seconds and CLS under 0.1

## Performance and UX Requirements

- Images should be optimized with modern formats such as WebP or AVIF where practical
- Lazy loading should be enabled for non-critical media
- CSS and JavaScript should be minified in production builds
- Avoid blocking scripts above the fold
- Keep the design mobile-first and responsive
- Ensure touch targets are at least 44px by 44px

## Third-Party Integration Requirements

Any embedded or connected service should be reviewed for both usability and compliance.

- Booking widgets must be accessible and secure
- Payment widgets must be PCI-compliant
- Chat widgets must not block keyboard navigation
- Analytics scripts should load asynchronously

## Next Recommended Enhancements

Suggested next steps:

- Persist admin lockout state in SQL Server
- Add image upload instead of URL-only image entry
- Add sorting controls for inventory lists
- Add inventory category management in admin
- Add package or pricing management for public services
- Add a richer gallery management workflow in admin
- Add audit fields or change history for inventory edits
- Move from demo token auth to stronger production-grade auth when needed

## Verification Checklist

When changing the app going forward, verify:

- API build passes
- React build passes
- Admin login works
- Inventory create/edit/delete flows work against the API
- Filters and pagination still behave correctly
- Inquiry creation and admin inquiry updates still work
- UI remains clean on both desktop and mobile

## Updated Product Instructions

These instructions reflect the latest requested direction for the Shenara Event Decor app and should be treated as active requirements for future implementation work.

### Public Site Navigation and Branding

- Remove the visible admin link from the public website navigation
- Admin access should still be possible privately by directly visiting the admin route
- Add the Shenara logo from the local `assets` folder to the public site branding
- Use the logo consistently in the public header and any appropriate brand areas

### Public Service Detail Pages

The Services section should no longer behave like static cards only.

Required behavior:

- `Back Arch Styling` should open a dedicated page
- That page should display the images from the `Back Arch Styling` folder
- Each image should have a polished, event-appropriate description
- `Balloon Decor` should open a dedicated page
- That page should display the images from the `Balloon Decor` folder
- Each image should have a polished, event-appropriate description

These pages should feel like real visual service showcases, not placeholder galleries.

### Gallery Detail Pages

- Each gallery item should link to its own detail page
- Clicking a gallery section or gallery image should route to a dedicated page
- The destination page should present the image or image set in a more immersive format with descriptive supporting copy

### Inquiry Admin Experience

- Fix the inquiries page status filter so it behaves correctly and consistently in the admin portal
- Make sure inquiry status filtering stays accurate after status updates, refreshes, and view changes

### Inquiry Protection and Abuse Controls

The inquiry flow should prevent repeated spam or duplicate submissions.

Required protections:

- Limit the number of inquiry submissions allowed per day for the same credentials or identifying details
- Treat repeated submissions with the same name, email, phone, and matching inquiry content on the same day as duplicates
- Do not allow repeated submission of the same inquiry details on the same day
- Track repeated submissions from the same IP address on the same day
- Add rate limiting for API requests from the same IP address
- Block or throttle repeated abuse after more than 3 attempts where applicable

For implementation planning, treat inquiry abuse prevention as both a UI and API concern.

### Admin and API Security Hardening

The current admin security model is not sufficient for production and must be upgraded.

Known issues to address:

- Admin credentials and token are hardcoded in configuration
- The admin token is stored in `localStorage`
- API startup does not yet include production-grade HTTPS, HSTS, CSP, rate limiting, or CSRF protections

Required direction:

- Remove hardcoded production credentials
- Use environment-based secrets management
- Replace lightweight token handling with a stronger authenticated session strategy
- Add persistent admin user storage
- Use password hashing
- Support session expiry and secure re-authentication
- Add persistent lockout tracking instead of memory-only lockout
- Enforce HTTPS and HSTS in production
- Add Content Security Policy support
- Add API rate limiting
- Add CSRF protection where relevant
- Avoid exposed admin surfaces and default credentials in production

### Accessibility Implementation Requirements

The app must implement the accessibility rules already documented in this plan, not only list them.

Immediate implementation priorities:

- Replace placeholder-only form inputs with real `<label>` and `id` associations
- Add a "Skip to content" link
- Add `aria-live` support for success and error messages
- Add `aria-describedby` relationships for validation messages
- Ensure accessible modal, menu, and accordion behavior where those patterns exist
- Verify keyboard-only navigation across the public site and admin portal

### Legal and Business Content Pages

The public website needs more business-complete content.

Required pages or content areas:

- Privacy Policy
- Accessibility Statement
- Terms of Service when appropriate
- Contact page or footer area with valid business information

If commerce is later introduced:

- Add refund or return policy content

### SEO Requirements

The site needs a more complete SEO setup.

Required additions:

- Unique page titles per route
- Meta descriptions per route
- Open Graph metadata
- Canonical URLs
- Structured data using JSON-LD for business information
- `robots.txt`
- XML sitemap

### Inventory Management Expansion

The inventory module should continue beyond the current MVP.

Missing features to plan for:

- Real image upload flow instead of URL-only entry
- Category management in admin
- Sorting controls in inventory views
- Audit trail or history for inventory edits
- Low-stock notifications or alerts

### Database and Deployment Maturity

The database lifecycle needs to move beyond local development shortcuts.

Required direction:

- Replace `EnsureCreated()` usage with proper Entity Framework migrations
- Use a migration-based schema workflow across environments
- Keep seeding logic compatible with migration-based deployments

## Implementation Priorities

Recommended order for upcoming work:

1. Remove public admin link and add logo from assets
2. Add service detail pages and gallery detail pages
3. Add inquiry duplicate prevention and IP-based rate limiting
4. Improve admin authentication and production security posture
5. Implement accessibility fixes in the actual UI
6. Add legal/business pages
7. Add SEO metadata, sitemap, and robots support
8. Expand inventory operations with uploads, sorting, and audit history
9. Move fully to migrations-based database lifecycle

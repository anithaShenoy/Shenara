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

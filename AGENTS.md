# Shenara Event Decor - Agent Guide

## Product Vision

Shenara Event Decor is a professional event decor business website with two main experiences:

1. A public-facing marketing website for external customers.
2. An admin portal for staff to manage decor inventory and visual assets.

The product should feel polished, warm, premium, and trustworthy. The public site should help customers imagine balloon decor, back arches, drop cloths, themed setups, and custom event styling for birthdays, baby showers, engagements, corporate events, and celebrations. The admin portal should feel efficient, clear, and business-focused.

## Technology Stack

- Frontend: React
- Backend: ASP.NET Core Web API
- Database: SQL Server
- Suggested ORM: Entity Framework Core
- Suggested authentication: ASP.NET Core Identity or JWT-based authentication
- Suggested image handling: store image metadata in SQL Server and image files in local storage, Azure Blob Storage, or another object storage service

## Application Areas

### Public Website

The public website is for customers exploring Shenara Event Decor services.

Core pages or sections:

- Home
- Services
- Gallery
- Packages or Decor Categories
- About
- Contact or Booking Inquiry

Services to highlight:

- Balloon decor
- Balloon garlands
- Balloon arches
- Back arches
- Drop cloth backdrops
- Custom color themes
- Event-specific decor packages
- Personalized setup and customization

Public UI expectations:

- Use high-quality imagery as a first-class part of the design.
- Make the first screen clearly communicate Shenara Event Decor and its event decor services.
- Avoid generic SaaS or corporate styling.
- Use a refined event-business visual style: elegant spacing, strong photography, clear service cards, and tasteful accent colors.
- Design for mobile first, then scale up gracefully for desktop.
- Make calls to action obvious, such as "Request a Quote", "View Services", or "Browse Gallery".
- Avoid overly decorative layouts that reduce readability or make the site feel like a template.

### Admin Portal

The admin portal is for authenticated admin users managing inventory and decor assets.

Core admin features:

- Admin login
- Inventory dashboard
- Create, read, update, and delete inventory items
- Upload and manage images for inventory items
- Track quantities
- Categorize inventory items
- Mark inventory as active, inactive, damaged, or unavailable
- Search, filter, and sort inventory

Inventory examples:

- Balloons
- Balloon arches
- Back arches
- Drop cloths
- Stands
- Props
- Table decor
- Custom add-ons

Admin UI expectations:

- Prioritize clarity, speed, and repeated use.
- Use tables, filters, forms, dialogs, and image thumbnails.
- Make quantity status easy to scan.
- Use confirmation dialogs for destructive actions.
- Show loading, empty, error, and success states.
- Keep the admin design quieter and more operational than the public website.

## Suggested Repository Structure

Use this structure unless the existing project already has a different clear pattern:

```text
src/
  Shenara.Api/
    Controllers/
    Data/
    Dtos/
    Models/
    Services/
    Program.cs
  Shenara.Web/
    src/
      components/
      features/
        admin/
        public/
      lib/
      pages/
      styles/
    package.json
```

## Backend Guidance

Build the backend as an ASP.NET Core Web API.

Core entities:

- User or AdminUser
- InventoryItem
- InventoryCategory
- InventoryImage
- Service
- GalleryImage
- Inquiry or BookingRequest

Suggested `InventoryItem` fields:

- `Id`
- `Name`
- `Description`
- `CategoryId`
- `Quantity`
- `Status`
- `PrimaryImageUrl`
- `Color`
- `Size`
- `IsFeatured`
- `CreatedAt`
- `UpdatedAt`

Suggested API routes:

- `POST /api/auth/login`
- `GET /api/inventory`
- `GET /api/inventory/{id}`
- `POST /api/inventory`
- `PUT /api/inventory/{id}`
- `DELETE /api/inventory/{id}`
- `POST /api/inventory/{id}/images`
- `GET /api/services`
- `GET /api/gallery`
- `POST /api/inquiries`

Backend implementation standards:

- Use DTOs for API input and output.
- Validate incoming data.
- Do not expose database entities directly when a DTO is more appropriate.
- Use async database operations.
- Keep controllers thin and move business logic into services where useful.
- Use migrations for database schema changes.
- Keep connection strings in configuration, not hardcoded in source files.
- Add CORS configuration for the React frontend during development.

## Database Guidance

Use SQL Server as the source of truth. Connect to local sql server db and make a db called Shenara

Important data requirements:

- Inventory items must support images and quantity tracking.
- Categories should be normalized so admins can filter and maintain inventory cleanly.
- Public gallery and services can either be managed from the database or seeded initially.
- Booking inquiries should be stored with customer contact details, event date, event type, message, and status.

Use sensible indexes for:

- Inventory category
- Inventory status
- Featured items
- Inquiry status
- Event date

## React Guidance

Build the frontend in React with a professional production mindset.

Recommended patterns:

- Use feature folders for admin and public experiences.
- Use reusable components for buttons, forms, inputs, tables, cards, modals, image uploaders, and empty states.
- Use a central API client.
- Keep data fetching logic organized by feature.
- Use route protection for admin pages.
- Keep styling consistent through shared design tokens or CSS variables.

Public website design guidance:

- Start with the actual customer experience, not a generic landing-page template.
- Show real decor categories and service examples prominently.
- Use elegant image-forward layouts.
- Make customization feel central to the brand.
- Keep copy concise, warm, and event-focused.

Admin portal design guidance:

- Use a dashboard layout with navigation.
- Use compact, readable tables and forms.
- Use thumbnails for inventory images.
- Show quantity badges or status indicators.
- Avoid oversized hero sections in admin screens.

## UI Quality Bar

The UI should look like it was designed by a professional UI developer for an event decor business.

Important qualities:

- Elegant but usable
- Responsive across mobile, tablet, and desktop
- Clear hierarchy
- Strong image presentation
- Consistent spacing
- Accessible color contrast
- Polished forms and buttons
- Useful loading and empty states
- No overlapping text or broken mobile layouts

Avoid:

- Generic placeholder-heavy pages
- One-note color palettes
- Oversized marketing sections in the admin portal
- Text that explains how the UI works inside the UI itself
- Layouts that depend on perfect image dimensions
- Hardcoded demo-only behavior where real data flow is expected

## Development Workflow

Before making changes:

- Inspect the existing project structure.
- Follow established naming, styling, and architecture.
- Keep changes scoped to the requested feature.

When adding backend features:

- Add or update models, DTOs, services, controllers, and migrations as needed.
- Verify the API builds.
- Add validation and useful error responses.

When adding frontend features:

- Build the real screen or workflow, not only static mockups.
- Check mobile and desktop behavior.
- Wire screens to the API or clearly isolate mock data when the backend is not ready.

When finished:

- Run relevant builds or tests when available.
- Mention any commands that could not be run.
- Summarize the files changed and the behavior added.


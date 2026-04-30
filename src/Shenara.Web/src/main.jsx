import React, { useEffect, useMemo, useRef, useState } from 'react';
import { createRoot } from 'react-dom/client';
import './styles.css';

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5189/api';

const fallbackServices = [
  {
    id: 1,
    name: 'Balloon Decor',
    description: 'Custom garlands, clusters, and color palettes styled around your celebration.',
    imageUrl: 'https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=1200&q=80',
    startingPrice: 'From $180'
  },
  {
    id: 2,
    name: 'Back Arch Styling',
    description: 'Statement arches with fabric, florals, signage, and premium balloon layering.',
    imageUrl: 'https://images.unsplash.com/photo-1527529482837-4698179dc6ce?auto=format&fit=crop&w=1200&q=80',
    startingPrice: 'From $240'
  },
  {
    id: 3,
    name: 'Custom Event Setups',
    description: 'Personalized decor concepts for birthdays, showers, engagements, and intimate events.',
    imageUrl: 'https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=1200&q=80',
    startingPrice: 'Custom quote'
  }
];

const fallbackGallery = [
  { id: 1, title: 'Blush birthday backdrop', eventType: 'Birthday', imageUrl: 'https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=900&q=80' },
  { id: 2, title: 'Reception detail', eventType: 'Reception', imageUrl: 'https://images.unsplash.com/photo-1519167758481-83f550bb49b3?auto=format&fit=crop&w=900&q=80' },
  { id: 3, title: 'Soft fabric setup', eventType: 'Shower', imageUrl: 'https://images.unsplash.com/photo-1464366400600-7168b8af9bc3?auto=format&fit=crop&w=900&q=80' }
];

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
    return 'The admin app could not reach the API. Please make sure the backend is running on http://localhost:5189.';
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
  const [route, setRoute] = useState(window.location.hash || '#home');

  useEffect(() => {
    const onHashChange = () => setRoute(window.location.hash || '#home');
    window.addEventListener('hashchange', onHashChange);
    return () => window.removeEventListener('hashchange', onHashChange);
  }, []);

  return route.startsWith('#admin') ? <AdminPortal route={route} /> : <PublicSite />;
}

function PublicSite() {
  const [services, setServices] = useState(fallbackServices);
  const [gallery, setGallery] = useState(fallbackGallery);
  const [inquiry, setInquiry] = useState({ customerName: '', email: '', phone: '', eventDate: '', eventType: '', message: '' });
  const [sent, setSent] = useState(false);

  useEffect(() => {
    request('/services').then(setServices).catch(() => setServices(fallbackServices));
    request('/gallery').then(setGallery).catch(() => setGallery(fallbackGallery));
  }, []);

  async function submitInquiry(event) {
    event.preventDefault();
    await request('/inquiries', { method: 'POST', body: JSON.stringify({ ...inquiry, eventDate: inquiry.eventDate || null }) });
    setInquiry({ customerName: '', email: '', phone: '', eventDate: '', eventType: '', message: '' });
    setSent(true);
  }

  return (
    <div className="site-shell">
      <header className="topbar">
        <a className="brand" href="#home">Shenara <span>Event Decor</span></a>
        <nav>
          <a href="#services">Services</a>
          <a href="#gallery">Gallery</a>
          <a href="#contact">Contact</a>
          <a className="admin-link" href="#admin">Admin</a>
        </nav>
      </header>

      <main>
        <section className="hero" id="home">
          <div className="hero-copy">
            <p className="eyebrow">Balloon decor, arches, backdrops, and custom styling</p>
            <h1>Shenara Event Decor</h1>
            <p className="hero-text">Elegant event decor for celebrations that deserve a beautiful focal point, thoughtful colors, and a setup that feels made for the moment.</p>
            <div className="hero-actions">
              <a className="primary-action" href="#contact">Request a Quote</a>
              <a className="secondary-action" href="#gallery">View Gallery</a>
            </div>
          </div>
          <div className="hero-image">
            <img src="https://images.unsplash.com/photo-1530103862676-de8c9debad1d?auto=format&fit=crop&w=1200&q=80" alt="Elegant balloon decor setup" />
          </div>
        </section>

        <section className="intro-band">
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
              <article className="service-card" key={service.id}>
                <img src={service.imageUrl} alt={service.name} />
                <div>
                  <span>{service.startingPrice}</span>
                  <h3>{service.name}</h3>
                  <p>{service.description}</p>
                </div>
              </article>
            ))}
          </div>
        </section>

        <section className="section gallery-section" id="gallery">
          <div className="section-heading">
            <p className="eyebrow">Gallery</p>
            <h2>Soft palettes, bold moments, clean details</h2>
          </div>
          <div className="gallery-grid">
            {gallery.map((image) => (
              <figure key={image.id}>
                <img src={image.imageUrl} alt={image.title} />
                <figcaption>{image.title}<span>{image.eventType}</span></figcaption>
              </figure>
            ))}
          </div>
        </section>

        <section className="contact-section" id="contact">
          <div className="contact-copy">
            <p className="eyebrow">Booking Inquiry</p>
            <h2>Tell us what you are celebrating</h2>
            <p>Share your event date, theme, colors, and favorite decor ideas. Shenara Event Decor can shape the package around your moment.</p>
          </div>
          <form className="inquiry-form" onSubmit={submitInquiry}>
            <input required placeholder="Name" value={inquiry.customerName} onChange={(event) => setInquiry({ ...inquiry, customerName: event.target.value })} />
            <input required type="email" placeholder="Email" value={inquiry.email} onChange={(event) => setInquiry({ ...inquiry, email: event.target.value })} />
            <input placeholder="Phone" value={inquiry.phone} onChange={(event) => setInquiry({ ...inquiry, phone: event.target.value })} />
            <input type="date" value={inquiry.eventDate} onChange={(event) => setInquiry({ ...inquiry, eventDate: event.target.value })} />
            <input placeholder="Event type" value={inquiry.eventType} onChange={(event) => setInquiry({ ...inquiry, eventType: event.target.value })} />
            <textarea required placeholder="Tell us about your colors, venue, and decor ideas" value={inquiry.message} onChange={(event) => setInquiry({ ...inquiry, message: event.target.value })} />
            <button type="submit">Send Inquiry</button>
            {sent && <p className="success-note">Inquiry sent. We will be in touch soon.</p>}
          </form>
        </section>
      </main>
    </div>
  );
}

function AdminPortal({ route }) {
  const [token, setToken] = useState(localStorage.getItem('shenara-admin-token') || '');

  if (!token) {
    return <LoginScreen onLogin={setToken} />;
  }

  return <AdminDashboard route={route} token={token} onLogout={() => { localStorage.removeItem('shenara-admin-token'); setToken(''); }} />;
}

function LoginScreen({ onLogin }) {
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
      localStorage.setItem('shenara-admin-token', result.token);
      onLogin(result.token);
    } catch (error) {
      if (error.status === 429) {
        const lockedUntil = error.body?.lockedUntilUtc ? new Date(error.body.lockedUntilUtc).toLocaleTimeString() : null;
        setLockoutMessage(lockedUntil
          ? `Too many failed attempts. Admin login is locked until ${lockedUntil}.`
          : 'Too many failed attempts. Admin login is temporarily locked.');
      }
      else if (error.status === 401) {
        setError(`Could not sign in. ${error.body?.remainingAttempts ?? 0} attempt(s) remaining.`);
      }
      else {
        setError(getRequestErrorMessage(error, 'Could not sign in with those credentials.'));
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="admin-login">
      <form className="login-panel" onSubmit={login}>
        <a className="brand" href="#home">Shenara <span>Event Decor</span></a>
        <h1>Admin Portal</h1>
        <p className="login-copy">Protected access for inventory, inquiries, and booking operations.</p>
        <input placeholder="Username" value={credentials.username} onChange={(event) => setCredentials({ ...credentials, username: event.target.value })} />
        <input type="password" placeholder="Password" value={credentials.password} onChange={(event) => setCredentials({ ...credentials, password: event.target.value })} />
        <button type="submit" disabled={isSubmitting}>{isSubmitting ? 'Checking Access' : 'Sign In'}</button>
        {error && <p className="error-note">{error}</p>}
        {lockoutMessage && <p className="error-note">{lockoutMessage}</p>}
      </form>
    </div>
  );
}

function AdminDashboard({ route, token, onLogout }) {
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
  const [lastViewed, setLastViewed] = useState(localStorage.getItem('shenara-inquiries-last-viewed') || '');
  const activeView = route === '#admin/inquiries' ? 'inquiries' : 'inventory';

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
      localStorage.setItem('shenara-inquiries-last-viewed', viewedAt);
      setLastViewed(viewedAt);
    }
  }, [activeView, inquiries.length]);

  async function refreshItems() {
    request('/inventory?page=1&pageSize=12').then(setInventoryPage).catch(() => setInventoryPage({
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
    request('/inquiries', { headers: { Authorization: `Bearer ${token}` } }).then(setInquiries).catch(() => setInquiries([]));
  }

  return (
    <div className="admin-shell">
      <aside className="admin-sidebar">
        <a className="brand" href="#home">Shenara <span>Event Decor</span></a>
        <nav>
          <a className={activeView === 'inventory' ? 'active' : ''} href="#admin">Inventory</a>
          <a className={activeView === 'inquiries' ? 'active with-badge' : 'with-badge'} href="#admin/inquiries">
            Inquiries
            {newInquiryCount > 0 && <span className="nav-badge">{newInquiryCount}</span>}
          </a>
          <a href="#home">Public Site</a>
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
          <InquiriesView token={token} inquiries={inquiries} onRefresh={refreshInquiries} />
        ) : (
          <InventoryView token={token} inventoryPage={inventoryPage} setInventoryPage={setInventoryPage} />
        )}
      </main>
    </div>
  );
}

function InventoryView({ token, inventoryPage, setInventoryPage }) {
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

    await request(`/inventory/${id}`, { method: 'DELETE', headers: { Authorization: `Bearer ${token}` } });
    setMessage('Inventory item deleted.');
    await refreshInventory(inventoryPage.page);
  }

  return (
        <section className="admin-grid">
          <form className="inventory-form" onSubmit={save} ref={formRef}>
            <h2>{editingId ? 'Edit Inventory' : 'Add Inventory'}</h2>
            {editingId && (
              <div className="edit-banner">
                <strong>Editing item</strong>
                <span>{editingLabel}</span>
              </div>
            )}
            <input ref={nameInputRef} required placeholder="Item name" value={form.name} onChange={(event) => setForm({ ...form, name: event.target.value })} />
            <select value={form.inventoryCategoryId} onChange={(event) => setForm({ ...form, inventoryCategoryId: event.target.value })}>
              {categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}
            </select>
            <textarea placeholder="Description" value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} />
            <div className="form-pair">
              <input type="number" min="0" placeholder="Total qty" value={form.totalQuantity} onChange={(event) => setForm({ ...form, totalQuantity: event.target.value })} />
              <input type="number" min="0" placeholder="Available qty" value={form.availableQuantity} onChange={(event) => setForm({ ...form, availableQuantity: event.target.value })} />
            </div>
            <div className="form-pair">
              <input type="number" min="0" placeholder="Low stock alert" value={form.minimumStockAlert} onChange={(event) => setForm({ ...form, minimumStockAlert: event.target.value })} />
              <select value={form.status} onChange={(event) => setForm({ ...form, status: event.target.value })}>
                {inventoryStatusOptions.map((option) => <option key={option} value={option}>{option}</option>)}
              </select>
            </div>
            <input placeholder="Image URL" value={form.primaryImageUrl} onChange={(event) => setForm({ ...form, primaryImageUrl: event.target.value })} />
            <div className="form-pair">
              <input placeholder="Color" value={form.color} onChange={(event) => setForm({ ...form, color: event.target.value })} />
              <input placeholder="Size" value={form.size} onChange={(event) => setForm({ ...form, size: event.target.value })} />
            </div>
            <div className="form-pair">
              <input placeholder="Storage location" value={form.storageLocation} onChange={(event) => setForm({ ...form, storageLocation: event.target.value })} />
              <input type="number" min="0" step="0.01" placeholder="Rental price" value={form.rentalPrice} onChange={(event) => setForm({ ...form, rentalPrice: event.target.value })} />
            </div>
            <label className="check-row">
              <input type="checkbox" checked={form.isFeatured} onChange={(event) => setForm({ ...form, isFeatured: event.target.checked })} />
              Featured public item
            </label>
            <button type="submit" disabled={isSaving || isLoadingEdit}>{isSaving ? 'Saving Changes' : editingId ? 'Save Changes' : 'Add Item'}</button>
            {editingId && <button className="quiet-button" type="button" onClick={() => { setEditingId(null); setEditingLabel(''); setForm(emptyInventory); setError(''); setMessage('Edit canceled.'); }}>Cancel Edit</button>}
            {message && <p className="success-note">{message}</p>}
            {error && <p className="error-note">{error}</p>}
          </form>

          <section className="inventory-panel">
            <div className="inventory-panel-header">
              <div className="panel-title">
                <h2>Inventory Directory</h2>
                <p>Browse the full catalog with server-backed paging so larger inventory sets stay fast and tidy.</p>
              </div>
            </div>
            <div className="inventory-tools">
              <input placeholder="Search inventory" value={query} onChange={(event) => setQuery(event.target.value)} />
              <select value={categoryFilter} onChange={(event) => setCategoryFilter(event.target.value)}>
                <option value="">All categories</option>
                {categories.map((category) => <option key={category.id} value={category.id}>{category.name}</option>)}
              </select>
              <select value={colorFilter} onChange={(event) => setColorFilter(event.target.value)}>
                <option value="">All colors</option>
                {colors.map((color) => <option key={color} value={color}>{color}</option>)}
              </select>
              <select value={status} onChange={(event) => setStatus(event.target.value)}>
                <option value="">All statuses</option>
                <option>Active</option>
                <option>Inactive</option>
                <option>Damaged</option>
                <option>Unavailable</option>
              </select>
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

function InquiriesView({ token, inquiries, onRefresh }) {
  const [status, setStatus] = useState('');
  const [message, setMessage] = useState('');

  const visibleInquiries = useMemo(() => {
    return inquiries.filter((inquiry) => !status || inquiry.status === status);
  }, [inquiries, status]);

  async function updateStatus(id, nextStatus) {
    await request(`/inquiries/${id}/status`, {
      method: 'PATCH',
      headers: { Authorization: `Bearer ${token}` },
      body: JSON.stringify({ status: nextStatus })
    });
    setMessage('Inquiry status updated.');
    onRefresh();
  }

  return (
    <section className="inquiries-panel">
      <div className="inventory-tools">
        <div className="panel-title">
          <h2>Customer Requests</h2>
          <p>Review event details, contact customers, and move each inquiry through the booking workflow.</p>
        </div>
        <select value={status} onChange={(event) => setStatus(event.target.value)}>
          <option value="">All statuses</option>
          <option>New</option>
          <option>Contacted</option>
          <option>Quoted</option>
          <option>Booked</option>
          <option>Completed</option>
          <option>Cancelled</option>
        </select>
      </div>

      {message && <p className="success-note">{message}</p>}

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
              <select value={inquiry.status} onChange={(event) => updateStatus(inquiry.id, event.target.value)}>
                <option>New</option>
                <option>Contacted</option>
                <option>Quoted</option>
                <option>Booked</option>
                <option>Completed</option>
                <option>Cancelled</option>
              </select>
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

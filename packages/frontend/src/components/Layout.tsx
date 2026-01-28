import { Outlet, Link } from 'react-router-dom'

function Layout() {
  return (
    <div className="layout">
      <header style={{ padding: '1rem', borderBottom: '1px solid #ddd' }}>
        <nav style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
          <Link to="/" style={{ fontWeight: 'bold', fontSize: '1.2rem' }}>
            MA Election Results
          </Link>
          <Link to="/elections">Elections</Link>
          <Link to="/reports">Reports</Link>
        </nav>
      </header>
      <main style={{ padding: '1rem' }}>
        <Outlet />
      </main>
    </div>
  )
}

export default Layout

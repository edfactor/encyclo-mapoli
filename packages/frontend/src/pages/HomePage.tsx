import { Link } from 'react-router-dom'

function HomePage() {
  return (
    <div>
      <h1>Massachusetts Election Results</h1>
      <p>
        Explore historical election data from the Commonwealth of Massachusetts.
      </p>
      <div style={{ marginTop: '2rem', display: 'flex', gap: '1rem' }}>
        <Link
          to="/elections"
          style={{
            padding: '0.75rem 1.5rem',
            background: '#0066cc',
            color: 'white',
            borderRadius: '4px',
          }}
        >
          Browse Elections
        </Link>
        <Link
          to="/reports"
          style={{
            padding: '0.75rem 1.5rem',
            background: '#666',
            color: 'white',
            borderRadius: '4px',
          }}
        >
          View Reports
        </Link>
      </div>
    </div>
  )
}

export default HomePage

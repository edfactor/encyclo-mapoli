function ReportsPage() {
  return (
    <div>
      <h1>Reports</h1>
      <p>Analyze election data with historical trends and geographic breakdowns.</p>
      <div style={{ marginTop: '1rem', display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
        <div
          style={{
            padding: '1.5rem',
            background: '#f5f5f5',
            borderRadius: '4px',
            flex: '1 1 200px',
          }}
        >
          <h2 style={{ fontSize: '1.1rem' }}>Historical Trends</h2>
          <p style={{ color: '#666', fontSize: '0.9rem' }}>
            Compare results across elections over time.
          </p>
        </div>
        <div
          style={{
            padding: '1.5rem',
            background: '#f5f5f5',
            borderRadius: '4px',
            flex: '1 1 200px',
          }}
        >
          <h2 style={{ fontSize: '1.1rem' }}>Geographic Analysis</h2>
          <p style={{ color: '#666', fontSize: '0.9rem' }}>
            Results by city, town, ward, and precinct.
          </p>
        </div>
        <div
          style={{
            padding: '1.5rem',
            background: '#f5f5f5',
            borderRadius: '4px',
            flex: '1 1 200px',
          }}
        >
          <h2 style={{ fontSize: '1.1rem' }}>Party Performance</h2>
          <p style={{ color: '#666', fontSize: '0.9rem' }}>
            How candidates and parties performed.
          </p>
        </div>
      </div>
    </div>
  )
}

export default ReportsPage

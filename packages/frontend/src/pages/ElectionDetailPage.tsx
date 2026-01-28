import { useParams } from 'react-router-dom'

function ElectionDetailPage() {
  const { id } = useParams()

  return (
    <div>
      <h1>Election Details</h1>
      <p>Election ID: {id}</p>
      <div
        style={{
          marginTop: '1rem',
          padding: '2rem',
          background: '#f5f5f5',
          borderRadius: '4px',
        }}
      >
        <p style={{ color: '#666' }}>
          Election results and details will be displayed here.
        </p>
      </div>
    </div>
  )
}

export default ElectionDetailPage

import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import HomePage from './pages/HomePage'
import ElectionsPage from './pages/ElectionsPage'
import ElectionDetailPage from './pages/ElectionDetailPage'
import ReportsPage from './pages/ReportsPage'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="elections" element={<ElectionsPage />} />
        <Route path="elections/:id" element={<ElectionDetailPage />} />
        <Route path="reports" element={<ReportsPage />} />
      </Route>
    </Routes>
  )
}

export default App

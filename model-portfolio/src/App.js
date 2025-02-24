import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Home from './pages/Home';
import AlbumPage from './pages/Album';
import Header from './components/Header'; // Add this import

function App() {
  return (
    <Router>
      <Header />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/album/:id" element={<AlbumPage />} />
      </Routes>
    </Router>
  );
}

export default App;
import { useParams } from 'react-router-dom';
import { albums } from '../data/albums';

const AlbumPage = () => {
  const { id } = useParams();
  const album = albums.find(a => a.id === parseInt(id));

  return (
    <div className="album-page">
      <h1>{album.title}</h1>
      <div className="photo-grid">
        {album.photos.map((photo, index) => (
          <img 
            key={index} 
            src={photo} 
            alt={`${album.title} ${index + 1}`} 
          />
        ))}
      </div>
    </div>
  );
};

export default AlbumPage;
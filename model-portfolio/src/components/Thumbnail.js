import { Link } from 'react-router-dom';

const Thumbnail = ({ album }) => {
  return (
    <div className="thumbnail-container">
      <Link to={`/album/${album.id}`}>
        <img 
          src={album.thumbnail} 
          alt={album.title} 
          className="thumbnail-image"
        />
        <div className="thumbnail-overlay">
          <h3 className="album-title">{album.title}</h3>
        </div>
      </Link>
    </div>
  );
};

export default Thumbnail;
import Thumbnail from './Thumbnail';
import { albums } from '../data/albums';

const AlbumGrid = () => {
  return (
    <div className="grid-container">
      {albums.map(album => (
        <Thumbnail key={album.id} album={album} />
      ))}
    </div>
  );
};

export default AlbumGrid;
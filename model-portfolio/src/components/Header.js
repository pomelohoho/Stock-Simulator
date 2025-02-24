import React from 'react';
import { Link } from 'react-router-dom';

const Header = () => {
  return (
    <header className="header">
      <nav>
        <Link to="/" className="site-title">
          Your Name | Portfolio
        </Link>
      </nav>
    </header>
  );
};

export default Header;
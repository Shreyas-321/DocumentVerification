import React from 'react';

const Card = ({ 
  children, 
  className = '', 
  title, 
  subtitle, 
  headerActions,
  footer,
  ...props 
}) => {
  return (
    <div className={`card shadow-sm border-0 ${className}`} {...props}>
      {(title || subtitle || headerActions) && (
        <div className="card-header bg-light d-flex justify-content-between align-items-center">
          <div>
            {title && <h5 className="card-title mb-0 fw-bold">{title}</h5>}
            {subtitle && <h6 className="card-subtitle text-muted mt-1">{subtitle}</h6>}
          </div>
          {headerActions && (
            <div className="d-flex align-items-center gap-2">
              {headerActions}
            </div>
          )}
        </div>
      )}
      <div className="card-body">
        {children}
      </div>
      {footer && (
        <div className="card-footer bg-light">
          {footer}
        </div>
      )}
    </div>
  );
};

export default Card;

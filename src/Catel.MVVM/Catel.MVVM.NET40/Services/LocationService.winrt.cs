﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocationService.android.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NETFX_CORE

namespace Catel.Services
{
    using System;
    using global::Windows.Devices.Geolocation;
    using Logging;

    public partial class LocationService
    {
        #region Fields
        private Geolocator _geoLocator;
        private Geopoint _lastKnownPosition;
        #endregion

        #region Methods
        private void OnGeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs e)
        {
            _lastKnownPosition = e.Position.Coordinate.Point;

            RaiseLocationChanged();
        }

        private void OnGeolocatorStatusChanged(Geolocator sender, StatusChangedEventArgs e)
        {
            Log.Info("Changed status to '{0}'", e.Status);

            RaiseLocationChanged();
        }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        protected override void Initialize()
        {
            // note: note required, see StartSensor
        }

        /// <summary>
        /// Gets the current location.
        /// </summary>
        /// <returns>
        /// The current location represented as <see cref="ILocation"/>. If no location is available, <c>null</c> will be returned.
        /// </returns>
        public override ILocation GetCurrentLocation()
        {
            var currentPosition = _lastKnownPosition;
            if (currentPosition == null)
            {
                return null;
            }
            
            var currentLocation = new Location(currentPosition.Position.Latitude, currentPosition.Position.Longitude,
                currentPosition.Position.Altitude);

            return currentLocation;
        }

        /// <summary>
        /// Starts the sensor.
        /// </summary>
        /// <returns><c>true</c> if the service started successfully; otherwise <c>false</c>.</returns>
        protected override bool StartSensor()
        {
            _geoLocator = new Geolocator();
            _geoLocator.DesiredAccuracy = PositionAccuracy.High;
            _geoLocator.MovementThreshold = 1d;

            _geoLocator.PositionChanged += OnGeolocatorPositionChanged;
            _geoLocator.StatusChanged += OnGeolocatorStatusChanged;
        
            return _geoLocator.LocationStatus != PositionStatus.Disabled;
        }

        /// <summary>
        /// Stops the location service so it's no longer retrieving data.
        /// </summary>
        protected override void StopSensor()
        {
            _geoLocator.PositionChanged -= OnGeolocatorPositionChanged;
            _geoLocator.StatusChanged -= OnGeolocatorStatusChanged;
            
            _geoLocator = null;
        }
        #endregion
    }
}

#endif
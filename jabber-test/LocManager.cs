using System;
using MonoTouch.CoreLocation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace jabbertest
{
	public class LocationManager
	{

		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

		public LocationManager (){
			this.locMgr = new CLLocationManager();
		}

		protected CLLocationManager locMgr;

		public CLLocationManager LocMgr{
			get { return this.locMgr; }
		}

		public void StartLocationUpdates ()
		{
			// We need the user's permission for our app to use the GPS in iOS. This is done either by the user accepting
			// the popover when the app is first launched, or by changing the permissions for the app in Settings

			if (CLLocationManager.LocationServicesEnabled) {

				LocMgr.DesiredAccuracy = 10; // sets the accuracy that we want in meters

				// Location updates are handled differently pre-iOS 6. If we want to support older versions of iOS,
				// we want to do perform this check and let our LocationManager know how to handle location updates.

				if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) {

					LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => {
						// fire our custom Location Updated event
						this.LocationUpdated (this, new LocationUpdatedEventArgs (e.Locations [e.Locations.Length - 1]));
					};

				} else {

					// this won't be called on iOS 6 (deprecated). We will get a warning here when we build.
					LocMgr.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
						this.LocationUpdated (this, new LocationUpdatedEventArgs (e.NewLocation));
					};
				}

				// Start our location updates
				LocMgr.StartUpdatingLocation ();

				// Get some output from our manager in case of failure
				LocMgr.Failed += (object sender, NSErrorEventArgs e) => {
					Console.WriteLine (e.Error);
				}; 

			} else {

				//Let the user know that they need to enable LocationServices
				Console.WriteLine ("Location services not enabled, please enable this in your Settings");

			}
		}

	}

	public class LocationUpdatedEventArgs : EventArgs
	{
		CLLocation location;

		public LocationUpdatedEventArgs(CLLocation location)
		{
			this.location = location;
		}

		public CLLocation Location
		{
			get { return location; }
		}
	}


}


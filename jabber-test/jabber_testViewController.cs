using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Collections;
using agsXMPP.protocol.iq.roster;
using System.Threading;
using MonoTouch.CoreLocation;

namespace jabbertest
{
	public partial class jabber_testViewController : UIViewController
	{
		XmppClientConnection xmpp = new XmppClientConnection("localhost");
		LocationManager Manager = null;

		private void XmppCon_OnReadXml(object sender, string xml)
		{   
			Console.WriteLine("Recv: " + xml);   
		}

		private void XmppCon_OnWriteXml(object sender, string xml)
		{
			Console.WriteLine("Send: " + xml);   
		}

		public jabber_testViewController () : base ("jabber_testViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			UIApplication.Notifications.ObserveDidBecomeActive ((sender, args) => {
				AppDelegate.Manager.LocationUpdated += HandleLocationChanged;
			});

			// whenever the app enters the background state, we unsubscribe from the event 
			// so we no longer perform foreground updates
			UIApplication.Notifications.ObserveDidEnterBackground ((sender, args) => {
				AppDelegate.Manager.LocationUpdated -= HandleLocationChanged;
			});

			string JID_Sender = "test_1";
			string Password = "pawcios";

			xmpp.AutoAgents = false;

			xmpp.Open(JID_Sender, Password);
			xmpp.OnLogin += new ObjectHandler(xmpp_OnLogin);

			xmpp.OnReadXml          += new XmlHandler(XmppCon_OnReadXml);
			xmpp.OnWriteXml        += new XmlHandler(XmppCon_OnWriteXml);

			//jabber.connection
			// Perform any additional setup after loading the view, typically from a nib.



			//agsXMPP.Jid JID = new Jid("test_2");
			//xmpp.MessageGrabber.Add(JID, new agsXMPP.Collections.BareJidComparer(), new MessageCB(GrabMessage), null);


		}

		public void xmpp_OnLogin(object sender)
		{
			Console.WriteLine("Logged In");  
			Console.WriteLine(xmpp.XmppConnectionState);

			xmpp.SendMyPresence ();

			agsXMPP.protocol.client.Message msg = new agsXMPP.protocol.client.Message();
			msg.Type = agsXMPP.protocol.client.MessageType.chat;
			msg.To = new Jid("test_2@localhost");
			msg.Body = "MICHALINA JEST SPOKO " + DateTime.Now.ToString();
			xmpp.Send (msg);

		}

		static void MessageCallBack
		(object sender, agsXMPP.protocol.client.Message msg, object data)
		{
			if (msg.Body != null)
			{                
				Console.WriteLine("{0}>> {1}", msg.From.User, msg.Body);
			}
		} 



		public void HandleLocationChanged (object sender, LocationUpdatedEventArgs e)
		{
			// handle foreground updates
			CLLocation location = e.Location;

			Console.WriteLine ("foreground updated");
		}
	

	}
}


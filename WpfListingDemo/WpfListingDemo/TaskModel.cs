using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WpfListingDemo
{
    public class TaskModel : ObservableObject
    {
		private int _id;

		public int ID
		{
			get { return _id; }
			set { SetProperty(ref _id, value); }
		}

		private string _website;

		public string Website
		{
			get { return _website; }
			set { SetProperty(ref _website, value); }
		}

		private string _size;

		public string Size
		{
			get { return _size; }
            set { SetProperty(ref _size, value); }
        }

		private string _keywords;

		public string Keywords
		{
			get { return _keywords; }
            set { SetProperty(ref _keywords, value); }
        }

		private string _proxy;

		public string Proxy
		{
			get { return _proxy; }
            set { SetProperty(ref _proxy, value); }
        }

		private string _billingProfile;

		public string BillingProfile
		{
			get { return _billingProfile; }
            set { SetProperty(ref _billingProfile, value); }
        }

		private string _status;

		public string Status
		{
			get { return _status; }
            set { SetProperty(ref _status, value); }
        }


		private bool _started;
		[JsonIgnore]
		public bool Started
		{
			get { return _started; }
			set { 
				SetProperty(ref _started, value);
				Status = value == true ? "Started" : "Idle";
			}
		}

    }
}

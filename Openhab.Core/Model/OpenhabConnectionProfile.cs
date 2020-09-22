using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHAB.Core.Model
{
	public class OpenhabConnectionProfile
	{
		public string Name
		{
			get; set;
		}

		public Uri Url
		{
			get; set;
		}

		public OpenHABConnectionType ConnectionType
		{
			get; set;
		}
	}
}

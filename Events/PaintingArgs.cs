using System;

namespace MapCore.Events
{
	public class PaintingArgs : EventArgs
	{
		public Nfa Nfa { get; set; }
		public Nfc Nfc { get; set; }
		public Nfe Nfe { get; set; }
		public Nfl Nfl { get; set; }
		public Nfp Nfp { get; set; }
		public Nfs Nfs { get; set; }
		public Nfw Nfw { get; set; }
		public Pvs Pvs { get; set; }
		public Qpf Qpf { get; set; }

		public bool CollectionChanged { get; set; }

		public PaintingArgs(
			Nfa nfa, 
			Nfc nfc,
			Nfe nfe,
			Nfl nfl,
			Nfp nfp,
			Nfs nfs, 
			Nfw nfw, 
			Pvs pvs,
			Qpf qpf, 
			bool changed)
		{
			Nfs = nfs;
			Nfc = nfc;
			Nfe = nfe;
			Nfl = nfl;
			Nfp = nfp;
			Nfa = nfa;
			Nfw = nfw;
			Pvs = pvs;
			Qpf = qpf;
			CollectionChanged = changed;
		}
	}
}

namespace SyslogNet.Client
{
	public enum Facility
	{
		KERN		= 0,
		USER		= 1,
		MAIN		= 2,
		DAEMON		= 3,
		SECURITY	= 4,
		SYSLOG		= 5,
		LPR		= 6,
		NEWS		= 7,
		UUCP		= 8,
		CRON		= 9,
		AUTHPRIV	= 10,
		FTP		= 11,

		/* Reserved for system use */
		NTP		= 12,
		AUDIT		= 13,
		ALERT		= 14,
		CRON2		= 15,

		LOCAL0		= 16,
		LOCAL1		= 17,
		LOCAL2		= 18,
		LOCAL3		= 19,
		LOCAL4		= 20,
		LOCAL5		= 21,
		LOCAL6		= 22,
		LOCAL7		= 23
	}
}
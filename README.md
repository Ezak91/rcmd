# rcmd
A simple c# library class to execute commands on a remote computer

***Requirements***
Add Reference to rcmd.dll and then

	using System.Management; 
	using rcmd;

***Using***

	Rcmd rcmd = new Rcmd("User","Password","MYDOMAIN","HOSTNAME",TRUE);
	int processID = rcmd.executeCommand("cmd /c shutdown -r",@"C:\Windows\system32");
	
***Wait for process to exit***
	
	while(rcmd.isProcessRunning(processID))
	{
	  Thread.Sleep(2000);
	}

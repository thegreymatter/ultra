ULTRA - The ultimate load testing utility
================================

Load testing and analysis tool
This is a utility that comes along with a nice web UI (User Interface) - Automatically runs load on your site and analyzes the results.
The utility uses :
 - JMeter
 - WebPageTest (future)
 - Selenium (future)

How to build your own package
-----------------------------
Requirements :
* Visual Studio 2012 (.net4 & mvc4)
* mongodb
* JMeter

Steps :
* MongoDB
 * Download an copy mongodb to `C:\utils\mongodb`
 * Copy the file 'config\mongodb.conf' (from your repo directory) to `C:\utils\mongodb`
 * Open the mongodb.conf file with a text editor, and make sure you have the log and db paths stated in the file (or change them to your preferred folders)
 * Open the command line and type `C:\utils\mongodb\bin\mongod --config "C:\utils\mongodb\mongodb.conf" --install` (This will install mongodb as a windows service. Omit the `--install` to just run mongodb not as a service).
 * Go to http://localhost:28017 - you should be seeing the mongodb admin panel
* JMeter
 * Download JMeter and copy to a directory of your choice
* Compiling
 * Open the solution with Visual Studio 2012 .net 4
 * Edit web.config file
  * `JMeterBatFile` - path to the jmeter batch file
  * `JmxFileArchive` - path to save the jmeter run scripts after running them
  * `OutputArchive` - path to save the output files
  * `JmxScripts` - path to the original jmx script files
* Run the utility - Ultra.Util.exe with required parameters.

Committers
----------
* gillyb - https://github.com/gillyb
* rabashani - https://github.com/rabashani

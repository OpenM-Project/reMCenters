<div align='center'>
<h1>reMCenters</h1>
<p>
  <img src='https://github.com/OpenM-Project/reMCenters/blob/master/MCenters/images/mcenter_5_icon.png?raw=true' alt='M Centers Icon' width="30%">
</p>
<p>A multi-method &amp; WIP unlocker for <em><a href="ms-windows-store://pdp/?ProductId=9NBLGGH2JHXJ">Minecraft for Windows</a></em>
</p>
</div>

## Usage

### Requirements

  - Windows 10 or above
  - An x64 or x86 CPU
### Optional Requirements
  - [Microsoft Visual C++ Redistributable](https://aka.ms/vs/17/release/vc_redist.x64.exe) to use certain Mod Options
  - Minecraft, should be already installed for certain Mod Options to work

<h3>Installation process</h3>
<ol>
    <li>Download <a href="ms-windows-store://pdp/?ProductId=9NBLGGH2JHXJ">Minecraft: Bedrock Edition Trial</a></li>
    <li>Download the <strong>latest</strong> version from <a href="https://mcenters.net/Downloads/M-Centers-8th-Edition/">reMCenters Website</a></li>
    <li>Extract the files</li>
    <li>Open <strong>M Centers.exe</strong></li>
    <li>Click <code>Install Modded DLL</code></li>
    <li>Click <code>Start</code></li>
</ol>

## How to compile reMCenters
- Clone or download the repository to your device
- [Download](https://visualstudio.microsoft.com/) Visual Studio 2022
- Download and install .Net Desktop Development Workload in Visual Studio 2022
- Open the 'M Centers 8.0.sln' file with Visual Studio
- Add `https://api.nuget.org/v3/index.json` as a Nuget Package Source in the Options
- Click 'Start' in the toolbar to build it

### Folder structure of the compiled program 
```
my-folder/                         # Root directory.
|- FluentWPF.dll                   # DLL file for FluentWPF library
|- M-Centers.exe                   # Main executable
|- M-Centers.exe.config            # Config file for M-Centers.exe
|- M Centers.pdb                   # PDB file for M Centers
|- MaterialDesignColors.dll        # DLL file for MaterialDesignColors library
|- MaterialDesignThemes.Wpf.dll    # DLL file for MaterialDesignThemes.Wpf library
|- MaterialDesignThemes.Wpf.xml    # XML file for MaterialDesignThemes.Wpf library
|- MCentersLibrary.dll             # DLL file for MCentersLibrary
|- MCentersLibrary.pdb             # PDB file for MCentersLibrary
```
## Contributors

 <a href = "https://github.com/OpenM-Project/reMCenters/graphs/contributors">
   <img src = "https://contrib.rocks/image?repo=OpenM-Project/reMCenters"/>
 </a>

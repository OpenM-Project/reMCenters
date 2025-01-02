<div align='center'>
<h1>reMCenters</h1>
<p>
  <img src='https://github.com/OpenM-Project/reMCenters/blob/master/MCenters/images/mcenter_5_icon.png?raw=true' alt='reMCenters Icon' width="30%">
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

## FAQ
<ol>
<li><p>Why does it give me Unsupported Version?</p>

The unsupported version error occurs when you are using the DLL Online method, and the DLL for your version of Windows is not found on the repository you are using (either custom or either the official M Centers DLL repo), to fix this you need to use the DLL Auto Patch method, which will take your system DLLs rather than online ones and patch them.
</li>
<li><p>Are permanent DLL methods dangerous?</p>

To answer this question, DLL methods are not dangerous at all, as long as you use official, patched DLLs, such as the M Centers DLL repo or you can use the DLL Auto Patch method, and not 3rd party DLLs from 3rd party repos, if they do cause damage, then you can always Uninstall the DLL method with Uninstall Hack, and switch to a non-permanent method such as the M Centers 5 method, the DLL (Non-Permanent) method, or the Hook method.
</li>
<li><p>Why does it give Access to the path 'C:\Windows\System32\Windows.ApplicationModel.Store.dll' is denied or something similar?</p>

This is either because you didn't run M Centers as administrator, or your antivirus is falsely flagging M Centers and not allowing it to be accessed. You can fix it by running M Centers as administrator, disabling your antivirus. If this still didn't fix your problem, press the Uninstall Hack button in M Centers, then restart your computer, and try Install Hack again.
</li>
<li><p>Why does it give me System.IO.FileNotFoundException?</p>

System.IO.FileNotFoundException occurs when the system cannot find files, this might be happening because you didn't extract the .zip file and are running M Centers standalone without any of its files from the .zip file, to fix it, you need to extract the .zip file
</li>
<li><p>Why doesn't the DLL Auto Patch method work while running Windows Event Viewer?</p>

Please close Windows Event Viewer while running M Centers using the mode, <strong>DLL Auto Patch</strong>, it will cause the system licensing dll <strong>Windows.ApplicationModel.Store.dll</strong> to become non-readable.
</li>
<li><p>Why does it give me an error like finding Records.txt or similar in China?</p>

If you live in China, check if you can visit raw.githubusercontent.com directly, but if you meet a flashback or get an error like cannot find file <strong>Records.txt</strong>, then try and  make a hosts record manually, like so:
<code>185.199.108.133               raw.githubusercontent.com</code>
</li>
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

## Works used
- [FluentWPF](https://github.com/sourcechord/FluentWPF) by [sourcechord](https://github.com/sourcechord/) - MIT License - Used for design.
- [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/) by [MaterialDesignInXAML](https://github.com/MaterialDesignInXAML) - MIT License - Used for design.
- [Zydis](https://github.com/zyantific/zydis) by [zyantific](https://github.com/zyantific/) - MIT License - Used for compiling patched DLL(s).
- [M Centers 8.0](https://github.com/tinedpakgamer/M-Centers-8.0/) by [tinedpakgamer](https://github.com/tinedpakgamer) - Custom License - This repo is the base for this repo.

#### Thank you all the developers above very much for their works to make this project possible.

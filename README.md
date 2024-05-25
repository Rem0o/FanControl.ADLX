# FanControl.ADLX [![Build status](https://ci.appveyor.com/api/projects/status/di3028g55s139rwt?svg=true)](https://ci.appveyor.com/project/Rem0o/fancontrol-adlx)

[![Donate](https://img.shields.io/badge/Donate-PayPal-blue.svg?style=flat&logo=paypal)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=N4JPSTUQHRJM8&currency_code=USD&source=url&item_name=Fan+Control)

Plugin for [FanControl](https://github.com/Rem0o/FanControl.Releases) (V113 and up) that provides support for ADLX compatible gpus (AMD RX 5000/6000/7000).
<br/><br/>
https://gpuopen.com/adlx/

## ADLX Bindings

I did my best to make functional bindings in this fork following the other examples/bindings : https://github.com/Rem0o/ADLX<br/>

## To install

* Download the latest [release](https://github.com/Rem0o/FanControl.ADLX/releases)
* (or) Download the latest binaries from [AppVeyor](https://ci.appveyor.com/project/Rem0o/fancontrol-adlx/build/artifacts)
* (or) Compile the solution.

And then

* Use the "install plugin" button from the settings page, select the zip.
* (or) place the (extracted) files in the "Plugins" folder.

## Notes

* AMD Software: Adrenalin Edition 22.7.1 driver or > required.
* Caveat: This will put Adrenaline/AMD driver in Manual tuning "Custom" mode, you can't have both automatic tuning (OC) and manual tuning enabled at the same time. 
* Wrapper from https://github.com/Rem0o/ADLX


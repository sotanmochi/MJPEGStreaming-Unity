# MJPEG Streaming for Unity

[Watch on Youtube](https://youtu.be/xKur_1pkDhg)

<img src="MJPEGStreaming-Unity.gif" width="50%">

## Tested Environment
- Unity 2018.4.21f1
- Windows 10
- Amazon GameLift

## Third party assets
以下のアセットをインポートする必要があります。  
You need to import the following assets.

- [LiteNetLib 0.8.3](https://github.com/RevenantX/LiteNetLib/releases/tag/v0.8.3)  
  Licensed under the MIT License. Copyright (c) Ruslan Pyrch.

- [Amazon GameLift CSharp Server SDK Version 3.4.0](https://s3-us-west-2.amazonaws.com/gamelift-release/GameLift_09_03_2019.zip)  
  Licensed under the Apache License 2.0. Copyright (c) Amazon.com, Inc. or its affiliates.  
  This product includes software developed by Amazon Technologies, Inc (http://www.amazon.com/).

- [UniRx 7.1.0](https://github.com/neuecc/UniRx/releases/tag/7.1.0)  
  This library is under the MIT License. Copyright (c) 2018 Yoshifumi Kawai.

## License
このプロジェクトは、サードパーティのアセットを除き、MIT Licenseでライセンスされています。  
This project is licensed under the MIT License excluding third party assets.

## How to setup server on Amazon GameLift

### Upload a custom server build to GameLift
AWS CLIを使ってサーバービルドをGameLiftにアップロードする
```
aws gamelift upload-build --name MJPEGStreamingServer --build-version v1.0.0 --build-root ./MJPEGStreaming-Unity/App/StreamingServerLinuxBuild --operating-system AMAZON_LINUX --region ap-northeast-1
```

### Create a fleet
AWSマネジメントコンソールでフリートを作成する

### Retrieves information about a fleet's instance
AWS CLIを使ってフリートのインスタンス情報を取得する
```
$ aws gamelift describe-instances --fleet-id "fleet-id"
{
    "Instances": [
        {
            "FleetId": "fleet-XXXXXXXXXXXXXXXXXXXXXXXX",
            "InstanceId": "i-XXXXXXXXXXXX",
            "IpAddress": "XX.XX.XX.XX",
            "DnsName": "ec2-XX-XX-XX-XX.ap-northeast-1.compute.amazonaws.com",
            "OperatingSystem": "AMAZON_LINUX",
            "Type": "c5.xlarge",
            "Status": "Active",
            "CreationTime": "2020-05-02T15:24:11.453000+09:00"
        }
    ]
}
```

## Tips

LiteNetLib - another reliable udp library. | Page 2 - Unity Forum  
https://forum.unity.com/threads/litenetlib-another-reliable-udp-library.414307/page-2

```
RevenantX

-----
RyuHayaboosa said: ↑

Hi RevenantX,

I need to stream images out of Unity to another application (preferably very quickly). 
Does your library handle files that are larger than the standard UDP packet size (64kb I think)?

Thanks!
-----

Yes. For ReliableOrdered and ReliableUnordered type of packets.
There some additional info:

-----
RevenantX said: ↑

Yes. But there some limitations. Maximum size of automaticly fragmented packet is 65535 * MTU. 
With mininum MTU - maximum data size will be around 32 mb. With MTU == 1500 - maximum data size will be 91 mb
-----

So if you need send larger data - better will be fragment image manually. 
Also when you fragment manually you can show nice progressbar in your game/program.

Aug 10, 2017
```

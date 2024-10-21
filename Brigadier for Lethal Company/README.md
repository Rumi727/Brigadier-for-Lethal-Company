# Brigadier for Lethal Company

[Brigadier.NET]: https://github.com/AtomicBlom/Brigadier.NET 

[Brigadier.NET]을 리썰 컴퍼니에 포팅한 모드입니다

## 설명

정말 기본적인 명령어와 execeute 명령어가 내장되어있으며\
그렇기에 Vector3 인수도 사용 가능하고, ~ ~ ~ 같은 상대 좌표도 당연히 사용할 수 있습니다\

또한 마인크래프트의 @e[type=enemy] @s 같은 엔티티 선택자 또한 사용할 수 있습니다

### 선택자 설명

@a : 모든 플레이어\
@e : 모든 엔티티 (플레이어, 적, 아이템) (인벤토리에 있는 아이템도 아이템으로 간주되니 주의하세요)\
@s : 자기 자신\
@p : 자신을 제외한 가장 가까운 플레이어\
@n : 자신을 제외한 가장 가까운 엔티티\
@r : 모든 플레이어 중 한 놈 랜덤\
[type=] 엔티티 종류 (앞에 '!'를 붙여서 부정으로도 사용 가능)\

예 :\
``@e[type=!player,type=!item]`` (``@e[type=!player|!item]`` 처럼 사용 가능)\
``@e[type=player|enemy]`` (플레이어 및 적만 가져옴)

엔티티 종류 : player, enemy, anomaly, item

[name=] 엔티티 이름 (앞에 '!'를 붙여서 부정으로도 사용 가능)\
예 : ``@e[name=Rumi727|jester|"특!수&문$자 및 띄어쓰기는 \"로 묶으면 됩니다."]``\
(``Rumi727`` 또는 ``jester`` 또는 ``특!수&문$자 및 띄어쓰기는 "로 묶으면 됩니다.`` 닉을 가진 플레이어 및 적 제스터만 가져옴)

[distance=] 거리\
예 : ``@e[type=!player,distance=..10]`` 플레이어를 제외한 주변 반경 10m 이내에 있는 모든 엔티티를 가져옴

[limit=] 개수 제한\
예 : ``@e[limit=1]`` 모든 엔티티 중 가장 첫번째 엔티티 하나를 가져옴

[death=false] 죽지 않은 엔티티만 선택\
[death=true] 죽은 엔티티만 선택

[sort=near] 가장 가까운 순으로 정렬\
[sort=far] 가장 먼 순으로 정렬\
[sort=random] 랜덤 정렬

## 커스텀 커맨드 추가

[ServerCommand](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/API/ServerCommand.cs)를 상속하면\
[BFLCNetworkHandler.OnNetworkSpawn](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/Networking/BFLCNetworkHandler.cs) 단계에서 인스턴스가 생성되며, Register 메소드가 자동으로 호출됩니다.

Register 메소드에서 ServerCommand.dispatcher에 노드를 등록하면 됩니다

인수 타입은 [RuniArguments](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/API/Arguments/RuniArguments.cs) 클래스를 통해 접근할 수 있습니다

현재 커맨드의 위치나 회전값 같은 것을 가져오려면\
[커맨드 소스](https://github.com/Rumi727/Brigadier-for-Lethal-Company/blob/main/Brigadier%20for%20Lethal%20Company/API/ServerCommandSource.cs)에 값이 담겨 있으니 그것을 참조하면 됩니다

자세한 사용법은 [Brigadier.NET] 문서를 참고하세요
자바이긴 하지만 [Fabric](https://fabricmc.net/wiki/tutorial:commands) 문서도 도움이 될 수 있습니다...ㅋㅋㅋㅋㅋㅋ

썬더스토어 릴리즈에 XML 문서도 포함되어 있으니 그것도 이용하면 도움 될거예요

### 주의!

코딩 실력의 한계로 인해 서버측 커스텀 커맨드를 추가하는 모드가 추가한 명령어가 목록에 표시되려면 클라이언트에도 모드를 깔아야합니다!
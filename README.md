# AR Defence
**[2022-1 경희대학교 캡스톤디자인1]**   
**증강현실을 활용한 멀티플레이어 디펜스 게임**

## 프로젝트 개요 및 소개
본 프로젝트는 증강현실을 활용한 멀티플레이어 디펜스 게임을 개발하는 것을 목표로 합니다.

### 게임 목표
사용자들은 증강현실 내에서 다가오는 몬스터들로부터 특정 오브젝트를 지켜야 합니다. 이 오브젝트는 넥서스(Nexus)라 불리며, 현실 세계의 좌표에 고정되어 존재합니다.   

사용자는 스마트폰의 스크린을 드래그하거나 터치함으로써 넥서스를 향해 다가오는 몬스터를 향해 공을 던지거나 공격을 가할 수 있습니다.   

공격을 받은 몬스터는 쓰러지게 되며, 몬스터가 넥서스를 파괴하기 전에 몬스터를 모두 무찌르면 플레이어들이 승리하게 됩니다. 반대로 몬스터가 넥서스의 체력을 모두 떨어뜨려 넥서스가 파괴되면 플레이어들은 패배합니다.   

### 멀티플레이
사용자들은 서로 다른 디바이스에서 동일한 WiFi에 무선 접속함으로써 동기화된 증강현실 환경을 경험할 수 있습니다.

어떤 디바이스에서 보더라도 현실 세계 기준으로 동일한 위치에서 다가오는 몬스터를 볼 수 있으며, 플레이어들끼리 서로의 시야 외부에서 다가오는 몬스터들을 확인해주며 협동해야 하도록 설계하였습니다.

### 몬스터 생성 위치
생성되는 몬스터는 여러 타입이 있습니다. 몬스터들은 각 타입에 따라 증강현실 내에서 생성되는 위치를 다른 방식으로 결정합니다.   

1. 플레이어가 바라보지 않는 위치
2. **Depth Image**를 활용한 위치

플레이어가 바라보지 않는 위치에 몬스터가 생성되는 이유는 멀티플레이 환경에서 플레이어 간 협동을 유도하기 위해서입니다.   
플레이어들은 끊임없이 몬스터가 어디서 다가오고 있는지를 확인해야 하며, 3차원의 현실 세계를 각자의 카메라의 시야각을 통해 바라봐야 하기 때문에 서로의 사각지대를 확인해주어야 합니다.    이러한 요소를 통해 멀티플레이 환경의 특성을 강조하고자 했습니다.

다른 타입의 몬스터는 카메라로부터 얻은 Depth Image를 통해 Depth가 급격히 변하는 지점의 평면에서 생성되도록 하였습니다.   
Depth가 급격히 변하는 지점은 일반적으로 물체가 식별되는 경계선으로, 평범한 바닥 평면이 아닌 특징이 있는 위치룰 고려하도록 하여 차별점을 두었습니다.

### 플레이어 아바타
카메라에 비친 동료 플레이어의 모습에 아바타가 적용됩니다.   
플레이어의 움직임에 맞춰 적용되는 모습이 업데이트됩니다.

![image](https://user-images.githubusercontent.com/72161388/174055129-92f9f265-b592-438f-be8a-595df697d16e.png)
![image](https://user-images.githubusercontent.com/72161388/174055140-bc0fb7ab-5908-4492-b599-761b17d03284.png)

***

## 게임 시나리오
### 멀티플레이 환경 구축
![image](https://user-images.githubusercontent.com/72161388/174053282-c701479b-6ec8-459d-a083-69e4f53fbd29.png)
![image](https://user-images.githubusercontent.com/72161388/174053295-f78a2d7f-ae8c-49c5-8e2d-7b33abde8b51.png)

Host가 될 디바이스는 게임 룸을 생성할 수 있습니다.   

Host가 생성한 게임 룸의 IP 주소를 입력함으로써 Client는 게임 룸에 접속할 수 있으며, 인원 모집이 확인되면 Host는 게임을 시작할 수 있습니다.

### 넥서스 생성
![image](https://user-images.githubusercontent.com/72161388/174054012-22d559d3-e1a6-45ea-91a8-529356254c09.png)

각 플레이어는 Anchor를 host/resolve함으로써 디바이스의 좌표계를 클라우드 앵커 좌표계로 동기화할 수 있습니다.   
좌표계가 성공적으로 동기화되면 Client의 화면에는 host가 생성한 곳과 동일한 위치에 넥서스가 생성됩니다.  

넥서스의 남은 체력 역시 확인할 수 있습니다.

### 몬스터 처치
![image](https://user-images.githubusercontent.com/72161388/174054614-952e14e9-0b21-4f9b-a80d-5356286a4ffc.png)

몬스터들은 사방에서 생성되어 넥서스를 향해 다가옵니다. 

화면에 표시된 공을 드래그하면 공이 날아가 충돌한 몬스터들을 처리할 수 있는데, 이 때 드래그의 속도나 방향에 따라 날아가는 공의 속도나 방향도 결정됩니다.   
따라서 신중한 컨트롤을 통해 공격을 진행해야 합니다.

## 프로젝트 구성원
|이름|학번|학과|학년|이메일|
|:---:|:---:|:---:|:---:|:---:|
|[송재혁](https://github.com/JaehyeokSong0)|2017103998|컴퓨터공학과|4|sjh17@khu.ac.kr|
|[장예원](https://github.com/Jangmanbo)|2018102226|컴퓨터공학과|4|wkddpdnjs99@khu.ac.kr|
|[전세계](https://github.com/jeonse3875)|2019102225|컴퓨터공학과|4|jeonse3875@khu.ac.kr|

## 버전 및 패키지 정보
- Unity 2020.3.32 LTS
- Netcode 1.0.0-pre8
- AR Foundation 4.1.9
- ARCore Extensions 1.30.0

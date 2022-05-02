Latest Ver. 
- 22.05.02

### [Dev]
- Aspect Ratio : 1920 * 1080 Portrait
- Unity 2020.3.32 LTS
- 만약 release된 apk가 강제종료되는 경우 project settings-player-configuration-scripting backend가 IL2CPP인지, Target Architectures에 ARM64가 체크되어 있는지 확인

### [Scripts]
#### BallManager
사용자가 던지는 구체에 대한 물리엔진 적용
- 탄성 적용하여 AR plane에 대해 튕기는 움직임 구현 필요
- 몬스터의 피격 판정 등에 사용
- Sphere의 component
- SpawnController GameObject의 component
#### HPManager
- 지켜야 할 object A(명칭 미정, 메인 스토리의 완성과 함께 추후 수정)의 남은 체력을 관리
- object A가 몬스터에게 피격당하면 체력이 깎임
- 각 사용자에게 UI의 형태로 상시 정보 제공
- object A와 Text_HP의 component
#### SpawnLocManager
몬스터가 생성될 위치 지정
#### MobXManager
SpawnLocManager로부터 받은 위치 정보를 통해 몬스터를 생성
- 지면으로부터의 높이, 사용자로부터의 거리 등을 고려한 개별 몬스터 생성
- 몇마리의 몬스터를 어느 정도 텀을 두고 생성할지 등을 전체적으로 관리
- 개별 몬스터에 대해서는 별도의 script 작성
    - 몬스터가 Object A 방향으로 다가오는 움직임 구현
    - 몬스터의 피격, 공격, 움직임, lifecycle, HP(체력바) 등 구현
#### Object A Manager (명칭 미정)
게임의 시작과 동시에 사용자가 위치 지정하면 지정된 plane에 놓임 (벽x, 바닥o)
- 게임 스타크래프트의 pylon과 같이 object A 근처에 일정 부분 지역이 형성됨 (원 or 다각형)
- user가 지정된 범위 벗어나면 object A의 hp가 깎임
- user간 위치 동기화 중요
#### ConnectionManager
user간 LAN 통신 지원
- cloud anchor 통해 위치 동기화

### [ETC]
- 간단한 스토리
- 게임 설명 및 툴팁 canvas
- 던지는 물체 swap 가능하게 하여 특성 부여 (ex. 공격력 차별점, 스플래시 효과, 설치형 등등)
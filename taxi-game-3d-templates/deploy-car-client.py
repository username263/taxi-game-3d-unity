import json
from template_generator import generate_stage

temp_group = generate_stage('./Template(Dev).xlsx', 'Car')

client_path = '../taxi-game-3d-client/Assets/_TaxiGame/Resources/Templates/Car.json'
with open(client_path, 'w', encoding='UTF-8-sig') as f:
    json.dump(temp_group, f, ensure_ascii=False)

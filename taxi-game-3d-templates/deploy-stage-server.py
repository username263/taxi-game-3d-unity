import requests
from template_generator import generate_stage

temp_group = generate_stage('./Template(Dev).xlsx', 'Stage')
res = requests.put('https://localhost:7170/Template/Stage', json=temp_group, verify=False)

print(res.status_code)

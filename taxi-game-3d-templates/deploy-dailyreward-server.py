import requests
from template_generator import generate_daily_reward

temp_group = generate_daily_reward('./Template(Dev).xlsx', 'DailyReward')
res = requests.put('https://localhost:7170/Template/DailyReward', json=temp_group, verify=False)

print(res.status_code)

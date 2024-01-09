using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class CustomerManager : MonoBehaviour
    {
        Customer[] customers;
        
        public int CurrentCustomerIndex
        {
            get;
            private set;
        } = -1;
        
        public int NextCustomerIndex
        {
            get;
            private set;
        } = -1;

        public bool WasCustomerTaken
        {
            get;
            private set;
        }

        void Start()
        {
            customers = new Customer[ClientManager.Instance.TemplateService.Customers.Count];
            NextCustomerIndex = Random.Range(0, customers.Length);
        }

        public IEnumerator TakeIn(Transform startPoint, PlayerCar car, bool isLast)
        {
            CurrentCustomerIndex = NextCustomerIndex;
            NextCustomerIndex = !isLast ? Random.Range(0, customers.Length) : -1;

            var customer = Spawn(startPoint.position, startPoint.rotation);
            var endPoint = car.SelectNearestPoint(startPoint.position);

            yield return null;
            customer.MoveTo(endPoint.position);
            while (customer.IsMoving)
                yield return null;

            customer.gameObject.SetActive(false);
            WasCustomerTaken = true;
        }

        public IEnumerator TakeOut(Transform endPoint, PlayerCar car)
        {
            var customer = customers[CurrentCustomerIndex];
            customer.gameObject.SetActive(true);
            var startPoint = car.SelectNearestPoint(endPoint.position);
            customer.transform.SetPositionAndRotation(
                startPoint.position, startPoint.rotation
            );

            yield return null;
            customer.MoveTo(endPoint.position);
            while (customer.IsMoving)
                yield return null;

            customer.gameObject.SetActive(false);
            CurrentCustomerIndex = -1;
            WasCustomerTaken = false;
        }

        Customer Spawn(Vector3 position, Quaternion rotation)
        {
            if (customers[CurrentCustomerIndex] == null)
            {
                var prefab = ClientManager.Instance.TemplateService.Customers[CurrentCustomerIndex].Prefab;
                var go = Instantiate(prefab);
                customers[CurrentCustomerIndex] = go.GetComponent<Customer>();
                customers[CurrentCustomerIndex].OnTakeIn += (sender, args) =>
                {
                    if (!WasCustomerTaken)
                        (sender as Customer).StopMove();
                };
            }

            customers[CurrentCustomerIndex].transform.SetPositionAndRotation(position, rotation);
            customers[CurrentCustomerIndex].gameObject.SetActive(true);

            return customers[CurrentCustomerIndex];
        }
    }
}

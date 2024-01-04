using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class CustomerManager : MonoBehaviour
    {
        [SerializeField]
        GameObject[] customerPrefabs;

        Customer[] customers;
        int customerIndex = -1;
        

        public bool WasCustomerTaken
        {
            get;
            private set;
        }

        void Start()
        {
            customers = new Customer[customerPrefabs.Length];
        }

        public IEnumerator TakeIn(Transform startPoint, PlayerCar car)
        {
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
            var customer = customers[customerIndex];
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
            customerIndex = -1;
            WasCustomerTaken = false;
        }

        Customer Spawn(Vector3 position, Quaternion rotation)
        {
            customerIndex = Random.Range(0, customers.Length);
            if (customers[customerIndex] == null)
            {
                var go = Instantiate(customerPrefabs[customerIndex]);
                customers[customerIndex] = go.GetComponent<Customer>();
                customers[customerIndex].OnTakeIn += (sender, args) =>
                {
                    if (!WasCustomerTaken)
                        (sender as Customer).StopMove();
                };
            }

            customers[customerIndex].transform.SetPositionAndRotation(position, rotation);
            customers[customerIndex].gameObject.SetActive(true);

            return customers[customerIndex];
        }
    }
}

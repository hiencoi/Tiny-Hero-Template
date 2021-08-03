using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.GridSlider
{
    [RequireComponent(typeof(GridSliderUI))]
    public class GridSliderDialog : Dialog
    {
        public int itemsPerPage;
        public GameObject gridItem;
        int m_totalPage = 1;
        int m_curPageIndex;
        GridSliderUI m_sliderUI;
        GridLayoutGroup m_configs;

        Vector3 m_nextPagePosition;
        Vector3 m_prevPagePosition;
        Vector3 m_viewPosition;

        bool m_nextBtnPressed;
        bool m_prevBtnPressed;
        bool m_pageMoving;

        bool m_isInited;

        protected GridSliderPageUI[] m_pagesOnScene;
        protected GameObject[] m_itemsOnScene;

        Coroutine m_pageNaviListenerCo;

        protected override void Awake()
        {
            base.Awake();
            m_sliderUI = GetComponent<GridSliderUI>();
            m_configs = m_sliderUI.configs;
        }

        public override void Close()
        {
            if (m_pageNaviListenerCo != null)
                StopCoroutine(m_pageNaviListenerCo);
            base.Close();
        }

        public override void Hide()
        {
            if (m_pageNaviListenerCo != null)
                StopCoroutine(m_pageNaviListenerCo);
            base.Hide();
        }

        public void DrawSlider(int totalItem, bool isOverride = false)
        {
            bool cond = isOverride ? isOverride : !m_isInited;

            if (totalItem > 0 && itemsPerPage > 0 && cond)
            {
                m_totalPage = Mathf.CeilToInt((float)totalItem / (float)itemsPerPage);

                m_pagesOnScene = new GridSliderPageUI[m_totalPage];
                m_itemsOnScene = new GameObject[totalItem];

                for (int i = 0; i < m_totalPage; i++)
                {
                    if (m_sliderUI.page)
                    {
                        GridSliderPageUI page = Instantiate(m_sliderUI.page, Vector3.zero, Quaternion.identity);

                        page.pageContent.cellSize = m_configs.cellSize;
                        page.pageContent.spacing = m_configs.spacing;
                        page.pageContent.startCorner = m_configs.startCorner;
                        page.pageContent.startAxis = m_configs.startAxis;
                        page.pageContent.childAlignment = m_configs.childAlignment;
                        page.pageContent.constraint = m_configs.constraint;
                        page.pageContent.constraintCount = m_configs.constraintCount;

                        if (m_sliderUI.mask)
                        {
                            page.transform.SetParent(m_sliderUI.mask);
                            page.transform.localPosition = Vector3.zero;
                            page.transform.localScale = Vector3.one;
                        }

                        m_pagesOnScene[i] = page;
                        var pageStartIndex = i * itemsPerPage;
                        var nextItems = (i * itemsPerPage) + itemsPerPage;
                        if (totalItem - nextItems < 0)
                        {
                            nextItems = totalItem;
                        }

                        Transform pageContent = page.pageContent.transform;

                        DrawContent(pageStartIndex, nextItems, pageContent);
                    }
                }
            }

            m_pageNaviListenerCo = StartCoroutine(PageNavigatorListener());
        }

        void DrawContent(int index, int totalItem, Transform pageContent)
        {
            for (int i = index; i < totalItem; i++)
            {
                if (gridItem)
                {
                    GameObject gridItemClone = Instantiate(gridItem, Vector3.zero, Quaternion.identity);

                    gridItemClone.transform.SetParent(pageContent);
                    gridItemClone.transform.localPosition = Vector3.zero;
                    gridItemClone.transform.localScale = Vector3.one;
                    m_itemsOnScene[i] = gridItemClone;

                    ItemHandle(gridItemClone, i);
                }
            }
        }

        public virtual void ItemHandle(GameObject gridItem, int index)
        {

        }

        public void UpdateAll()
        {
            if(m_itemsOnScene != null && m_itemsOnScene.Length > 0)
            {
                for (int i = 0; i < m_itemsOnScene.Length; i++)
                {
                    if(m_itemsOnScene[i] != null)
                    {
                        ItemHandle(m_itemsOnScene[i], i);
                    }
                }
            }
        }

        void PagePosInit()
        {
            if (m_isInited) return;

            if (m_pagesOnScene != null && m_pagesOnScene.Length > 0)
            {
                if (m_pagesOnScene[0] != null)
                {
                    m_viewPosition = Vector3.zero;

                    if (m_pagesOnScene[0].with > 0)
                    {
                        float navigatorOffet = 150f + m_configs.padding.left + m_configs.padding.right;
                        m_nextPagePosition = new Vector3(m_pagesOnScene[0].with + navigatorOffet, 0f, 0f);
                        m_prevPagePosition = new Vector3(-(m_pagesOnScene[0].with + navigatorOffet), 0f, 0f);

                        if (m_sliderUI)
                        {
                            if(m_sliderUI.mask)
                                m_sliderUI.mask.sizeDelta = new Vector2(m_pagesOnScene[0].with, m_pagesOnScene[0].height);

                            if (m_sliderUI.main)
                                m_sliderUI.main.sizeDelta = new Vector2(m_pagesOnScene[0].with + m_configs.padding.left + m_configs.padding.right,
                                    m_pagesOnScene[0].height + m_configs.padding.top + m_configs.padding.bottom);

                            if (m_totalPage > 1)
                            {
                                if (m_sliderUI.nextPageBtn && m_sliderUI.prevPageBtn)
                                {
                                    if (m_sliderUI.main)
                                    {
                                        m_sliderUI.nextPageBtn.transform.SetParent(m_sliderUI.main);
                                        m_sliderUI.nextPageBtn.transform.localPosition = m_nextPagePosition / 2;
                                        m_sliderUI.nextPageBtn.transform.localScale = Vector3.one;
                                        m_sliderUI.nextPageBtn.onClick.RemoveAllListeners();
                                        m_sliderUI.nextPageBtn.onClick.AddListener(() => NextPage());

                                        m_sliderUI.prevPageBtn.transform.SetParent(m_sliderUI.main);
                                        m_sliderUI.prevPageBtn.transform.localPosition = m_prevPagePosition / 2;
                                        m_sliderUI.prevPageBtn.transform.localScale = Vector3.one;
                                        m_sliderUI.prevPageBtn.onClick.RemoveAllListeners();
                                        m_sliderUI.prevPageBtn.onClick.AddListener(() => PrevPage());
                                    }
                                }
                            }
                            else
                            {
                                if (m_sliderUI.nextPageBtn && m_sliderUI.prevPageBtn)
                                {
                                    m_sliderUI.nextPageBtn.gameObject.SetActive(false);
                                    m_sliderUI.prevPageBtn.gameObject.SetActive(false);
                                }
                                }

                            if (m_sliderUI.closeBtn)
                            {
                                if (m_sliderUI.main)
                                {
                                    m_sliderUI.closeBtn.transform.SetParent(m_sliderUI.main);
                                    m_sliderUI.closeBtn.transform.localScale = Vector3.one;
                                    m_sliderUI.closeBtn.transform.localPosition = new Vector2(m_pagesOnScene[0].with / 2 - 20, m_pagesOnScene[0].height / 2 - 20);
                                    m_sliderUI.closeBtn.onClick.RemoveAllListeners();
                                    m_sliderUI.closeBtn.onClick.AddListener(() => Close());
                                }
                            }

                        }

                        for (int i = 1; i < m_pagesOnScene.Length; i++)
                        {
                            if (m_pagesOnScene[i] != null)
                            {
                                m_pagesOnScene[i].transform.localPosition = m_nextPagePosition;
                            }
                        }

                        Destroy(m_pagesOnScene[m_totalPage - 1].pageContent.GetComponent<ContentSizeFitter>());

                        m_pagesOnScene[m_totalPage - 1].pageContent.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(m_pagesOnScene[0].with, m_pagesOnScene[0].height);

                        if (m_pagesOnScene.Length == m_totalPage)
                            m_isInited = true;
                    }
                }
            }
        }

        /// <summary>
        /// Listener coroutine for animation of slider
        /// Loop every 0.02s
        /// </summary>
        IEnumerator PageNavigatorListener()
        {
            int step = 0;
            while (step < 5000)
            {
                PagePosInit();

                NextPageAnimation();
                PrevPageAnimation();
                step += 1;
                yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.02f));
                if (step >= 5000)
                {
                    step = 0;
                }
            }
        }

        /// <summary>
        /// Next page for button event
        /// </summary>
        void NextPage()
        {
            if (m_curPageIndex < (m_totalPage - 1) && !m_pageMoving)
            {
                m_pageMoving = true;
                m_curPageIndex++;
                m_nextBtnPressed = true;
            }
        }

        /// <summary>
        /// Prev page for button event
        /// </summary>
        void PrevPage()
        {
            if (m_curPageIndex > 0 && !m_pageMoving)
            {
                m_pageMoving = true;
                m_curPageIndex--;
                m_prevBtnPressed = true;
            }
        }

        /// <summary>
        /// Animation when next page event trigger
        /// </summary>
        void NextPageAnimation()
        {
            if (m_nextBtnPressed && !m_prevBtnPressed)
            {
                var prevPage = m_pagesOnScene[m_curPageIndex - 1];
                var nextPage = m_pagesOnScene[m_curPageIndex];
                prevPage.transform.localPosition = Vector2.MoveTowards(prevPage.transform.localPosition, m_prevPagePosition, Time.unscaledDeltaTime * 3500f);
                nextPage.transform.localPosition = Vector2.MoveTowards(nextPage.transform.localPosition, m_viewPosition, Time.unscaledDeltaTime * 3500f);
                if (Vector2.Distance(nextPage.transform.localPosition, m_viewPosition) <= 0.1f)
                {
                    nextPage.transform.localPosition = m_viewPosition;
                    m_nextBtnPressed = false;
                    m_pageMoving = false;
                }
            }
        }

        /// <summary>
        /// Animation when prev page event trigger
        /// </summary>
        void PrevPageAnimation()
        {
            if (m_prevBtnPressed && !m_nextBtnPressed)
            {
                var prevPage = m_pagesOnScene[m_curPageIndex + 1];
                var nextPage = m_pagesOnScene[m_curPageIndex];
                prevPage.transform.localPosition = Vector2.MoveTowards(prevPage.transform.localPosition, m_nextPagePosition, Time.unscaledDeltaTime * 3500f);
                nextPage.transform.localPosition = Vector2.MoveTowards(nextPage.transform.localPosition, m_viewPosition, Time.unscaledDeltaTime * 3500f);
                if (Vector2.Distance(nextPage.transform.localPosition, m_viewPosition) <= 0.1f)
                {
                    nextPage.transform.localPosition = m_viewPosition;
                    m_prevBtnPressed = false;
                    m_pageMoving = false;
                }
            }
        }
    }
}
